using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AnimalType
{
    Sheep,
    Dog, 
    Rabbit
}

public class AnimalHit
{
    [SerializeField]
    private AnimalType _animalType;
    private GameObject _gameObject;
    private Animal _animal;

    public AnimalType AnimalType => _animalType;

    public GameObject GameObject => _gameObject;

    public Animal Animal => _animal;

    public AnimalHit(AnimalType animalType, GameObject gameObject, Animal animal)
    {
        _animalType = animalType;
        _gameObject = gameObject;
        _animal = animal;
    }
}
public class Animal : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField] 
    private AudioClip _flyClip;
    [SerializeField]
    private AudioClip _creatureClip;
    [SerializeField]
    private Material _material;
    [SerializeField]
    private AnimalType _animalType;

    [SerializeField] 
    private Transform _model;
    [SerializeField] 
    private AnimalPool _pool;
    [SerializeField]
    private InteractionHandle _interactionHandle;
    [SerializeField]
    private InteractionHandle _squishHandle;
    
    private Vector3 _flyDirection;
    private float _groundAltitude = 0f;
    [SerializeField]
    private float _gravity = 1f;

    [SerializeField]
    private float _requiredCutness = 1.5f;

    [SerializeField]
    private float _minimalSpawnCutness = 0.75f;

    [SerializeField] private float _maxPettingDistance = 0.25f;
    
    private Coroutine _movementCoroutine;
    private bool _grabable = true;
    private bool _isFlying = false;
    private float _baseHeight;
    private BroadcastSender<Animal> _broadcastSender;
    private BroadcastSender<AnimalHit> _broadcastSenderHit;
    [SerializeField]
    private float _aggregateCutness = 0f;
    private float _lastCutness = 0f;
    private GameManager _gameManager;
    
    public AnimalType AnimalType => _animalType;

    public bool IsFlying => _isFlying;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.OnGameEnded += GameManagerOnGameEnded;
        _pool = FindObjectOfType<AnimalPool>();
        _broadcastSender = new BroadcastSender<Animal>();
        _broadcastSenderHit = new BroadcastSender<AnimalHit>();
        _baseHeight = 1f;
    }

    private void Update()
    {
        if (_grabable && !_interactionHandle.IsGrabbed)
        {
            Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up) ;
            if (Physics.Raycast(ray, out RaycastHit hit, 2000f))
            {
                float hitHeight = hit.point.y;
                float difference = hitHeight - transform.position.y;
                if (Mathf.Abs(difference)> Single.Epsilon)
                {
                    float magnitude = Mathf.Min(Time.deltaTime, Mathf.Abs(difference)) * Mathf.Sign(difference);
                    transform.position += Vector3.up * magnitude;
                }
     
            }
            else
            {
                Debug.Log("No hit :(");
            }
        }
    }

    private void GameManagerOnGameEnded()
    {
        _material.DOFloat(1f, "_GrayOut", 1f).onComplete += () =>
        {
            Destroy(this);
        };
    }

    private void OnEnable()
    {
        _interactionHandle.OnBeingGrabbed += InteractionHandleOnOnBeingGrabbed;
        _squishHandle.OnBeingGrabbed += SquishHandleOnBeingGrabbed;
        _squishHandle.OnGrabbed += SquishHandleOnGrabbed;
        _squishHandle.OnReleased += SquishHandleOnReleased;
        _interactionHandle.OnGrabbed += InteractionHandleOnGrabbed;
        _interactionHandle.OnReleased += InteractionHandleOnReleased;
        _grabable = true;
        _isFlying = false;
    }

    private void SquishHandleOnReleased(Interactor obj)
    {
        _model.DOScale(Vector3.one, 0.5f);
    }

    private void SquishHandleOnGrabbed(Interactor obj)
    {
        _lastCutness = 0f;
    }

    private void SquishHandleOnBeingGrabbed(TransformData[] obj)
    {
        if (_grabable)
        {
            Vector3 position = obj[0].Position;
            float distance = (position.y - transform.position.y)/_maxPettingDistance;
            distance *= distance;
            float cutness = 1 - distance;
            _aggregateCutness += Mathf.Clamp01(_lastCutness - cutness);
            _lastCutness = cutness;
            _model.localScale = new Vector3(_model.localScale.x, Mathf.Min(distance, _baseHeight), _model.localScale.z);
            if (cutness >= _minimalSpawnCutness && _aggregateCutness >= _requiredCutness)
            {
                _aggregateCutness = 0f;
                _audioSource.PlayOneShot(_creatureClip);
                SpawnAnother();
            }
        }
    }

    private void SpawnAnother()
    {
        if (_pool.TryGetAnimal(_animalType, out Animal newAnimal))
        {
            float radians = Random.Range(0, 1f)*2*Mathf.PI;
            float x = Mathf.Sin(radians);
            float z = Mathf.Cos(radians);
            Vector3 direction = new Vector3(x, 0, z).normalized;
            newAnimal.transform.position = transform.position;
            newAnimal.transform.DOMove(transform.position + (direction*0.15f), 1.5f);
        }
    }
    
    private void OnDisable()
    {
        _interactionHandle.OnBeingGrabbed -= InteractionHandleOnOnBeingGrabbed;
        _interactionHandle.OnGrabbed -= InteractionHandleOnGrabbed;
        _interactionHandle.OnReleased -= InteractionHandleOnReleased;
        _squishHandle.OnBeingGrabbed -= SquishHandleOnBeingGrabbed;
        _squishHandle.OnGrabbed -= SquishHandleOnGrabbed;
        _interactionHandle.OnReleased -= InteractionHandleOnReleased;
    }
    

    private void InteractionHandleOnReleased(Interactor obj)
    {
        _broadcastSender.ReleaseBroadcast();
        obj.InteractionLayer = InteractionLayer.Hands;
    }

    private void InteractionHandleOnGrabbed(Interactor obj)
    {
        _audioSource.PlayOneShot(_creatureClip);
        _broadcastSender.Send(transform.position, this);
        obj.InteractionLayer = InteractionLayer.Default;
    }

    private void InteractionHandleOnOnBeingGrabbed(TransformData[] args)
    {
        if (_grabable)
        {
            transform.position = args[0].Position;
            transform.rotation = args[0].Rotation;
            _broadcastSender.UpdateBroadcastPosition(transform.position);
        }
    }

    public void StartFly(Vector3 flyDirection)
    {
        _audioSource.PlayOneShot(_flyClip);
        _flyDirection = flyDirection;
        _isFlying = true;
        _broadcastSenderHit.Send(transform.position, new AnimalHit(_animalType, gameObject, this));
        _movementCoroutine = StartCoroutine(FlyCoroutine());
    }

    public void PlaceInSling()
    {
        _grabable = false;
        _broadcastSender.ReleaseBroadcast();
    }

    public void Return()
    {
        _pool.ReturnAnimal(this);
    }
    IEnumerator FlyCoroutine()
    {
        while (transform.position.y > _groundAltitude)
        {
            transform.position += _flyDirection * Time.deltaTime;
            _flyDirection += Vector3.down * Time.deltaTime * _gravity;
            _broadcastSenderHit.UpdateBroadcastPosition(transform.position);
            yield return null;
        }
        _broadcastSenderHit.ReleaseBroadcast();
        Return();
    }
}
