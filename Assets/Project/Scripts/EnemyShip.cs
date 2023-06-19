using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyShip : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;
    [SerializeField] 
    private AudioSource _source;

    [SerializeField] 
    private AudioClip _grumblingClip;
    [SerializeField]
    private AudioClip _confusionClip;
    [SerializeField]
    private AudioClip _defeatedlip;
    
    [SerializeField]
    private InteractionBounds _interactionBounds;

    [SerializeField]
    private Material _material;

    [SerializeField]
    private float _speed = 0.3f;

    [SerializeField] 
    private Mast _mast;



    [SerializeField] 
    private int[] _neededAnimals;

    [SerializeField] 
    private Vector2 _animalAmountRange;
    
    
    private Stack<Desitnation> _returnStack = new Stack<Desitnation>();
    [SerializeField]
    private Desitnation _currentDestination;
    private BroadcastReceiver<AnimalHit> _broadcastReceiver;
    private bool _isMoving;
    private LandingStop _landingStop;
    private GameManager _gameManager;
    private Coroutine _movementCoroutine;
    
    private void Awake()
    {
        _broadcastReceiver = new BroadcastReceiver<AnimalHit>(_interactionBounds);
        _broadcastReceiver.OnBroadcastReceived += BroadcastReceiverOnBroadcastReceived;
    }
    public void Initialize(EnemySpawnPoint enemySpawnPoint)
    {
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.OnGameEnded += GameManagerOnGameEnded;
        transform.position = enemySpawnPoint.transform.position;
        _currentDestination = enemySpawnPoint.Next;
        _returnStack.Push(enemySpawnPoint);
        
        SetAnimalsToReceive();
        _mast.VisualizeNeededAnimals(_neededAnimals);
        _isMoving = true;
        _movementCoroutine = StartCoroutine(FollowWaypoint());
    }

    private void GameManagerOnGameEnded()
    {
        Stop();
        _material.DOFloat(1f, "_GrayOut", 1f).onComplete += () =>
        {
            Destroy(this);
        };
    }

    private void SetAnimalsToReceive()
    {
        int amount = (int) Random.Range(_animalAmountRange.x, _animalAmountRange.y);
        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, _neededAnimals.Length);
            _neededAnimals[index]++;
        }
    }



    public void SetNewDestination(Desitnation destination)
    {
        _currentDestination = destination;
    }

    public void Land()
    {
        _source.PlayOneShot(_grumblingClip);
        Stop();
    }
    
    public void Stop()
    {
        _isMoving = false;
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
        }
        _currentDestination = null;
    }

    public void SetLanding(LandingStop landingStop)
    {
        SetNewDestination(landingStop);
        _landingStop = landingStop;
    }

    private void BroadcastReceiverOnBroadcastReceived(AnimalHit hit)
    {
        int index = (int) hit.AnimalType;
        if (_neededAnimals[index] > 0)
        {
            _neededAnimals[index]--;
            _mast.RemoveFlag(index);
            _particleSystem.Play();
        }

        if (NeedsMet())
        {
            GoAway();
        }
        else
        {
            _source.PlayOneShot(_confusionClip);
        }
        hit.Animal.Return();
    }
    
    private void GoAway()
    {
        Debug.Log(("Return stack " + _returnStack.Count));
        _source.PlayOneShot(_defeatedlip);
        if (_landingStop)
        {
            _landingStop.IsOccupied = false;
        }

        Stop();
        StartCoroutine(Return());
    }

    private bool NeedsMet()
    {
        foreach (var neededAnimal in _neededAnimals)
        {
            if (neededAnimal > 0)
            {
                return false;
            }
        }
        return true;
    }
    
    IEnumerator FollowWaypoint()
    {
        while (_isMoving)
        {
            Vector3 dir = _currentDestination.transform.position - transform.position;
            float magnitude = dir.magnitude;
            dir.Normalize();
            transform.forward = Utils.LerpVector(transform.forward, dir, Time.deltaTime);
            transform.position += transform.forward * Time.deltaTime * _speed;
            Desitnation previous = _currentDestination;
            if (_currentDestination.TryReach(magnitude, this))
            {
                _returnStack.Push(previous);
            }
            yield return null;
        }
    }
    IEnumerator Return()
    {
        bool keepReturning = true;
        _currentDestination = _returnStack.Pop();
        Debug.Log("Current destination " + _currentDestination.name);
        Assert.IsNotNull(_currentDestination);
        while (keepReturning)
        {
            Vector3 dir = _currentDestination.transform.position - transform.position;
            float magnitude = dir.magnitude;
            dir.Normalize();
            transform.forward = Utils.LerpVector(transform.forward, dir, 5*Time.deltaTime);
            transform.position += transform.forward * Time.deltaTime * _speed;
            if (_currentDestination.TryReach(magnitude, this, true))
            {
                Debug.Log("Return stack" + _returnStack.Count);
                keepReturning = _returnStack.TryPop(out _currentDestination);
            }

            yield return null;
        }
        _gameManager.OnGameEnded -= GameManagerOnGameEnded;
        _broadcastReceiver.OnBroadcastReceived -= BroadcastReceiverOnBroadcastReceived;
        Destroy(this.gameObject);
    }
}
