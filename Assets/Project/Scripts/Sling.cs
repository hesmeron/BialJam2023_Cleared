using System.Collections;
using UnityEngine;

public class Sling : MonoBehaviour
{
    [SerializeField]
    private InteractionHandle _interactionHandle;
    
    [SerializeField] 
    private Transform _target;

    [SerializeField]
    private Vector3 _drawOrigin;

    [SerializeField]
    private float _maxMagnitude;

    [SerializeField] private float _throwMagnitudeMultiplier = 25f;
    
    private Animal _animal;

    [SerializeField] 
    private InteractionBounds _interactionBounds;

    [SerializeField]
    private GameObject _indicatorPrefab;

    [SerializeField] 
    private int _indicatorCount;

    private Vector3 _flyDirection;
    
    private BroadcastReceiver<Animal> _animalReceiver;
    private Coroutine _relaxCoroutine;
    private GameObject[] _indicators;

    private void OnDrawGizmos()
    {
        if (_animal)
        {
            Gizmos.DrawLine(_animal.transform.position, _animal.transform.position + _flyDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawSphere(_drawOrigin, 0.1f);
    }

    void Awake()
    {
        _animalReceiver = new BroadcastReceiver<Animal>(_interactionBounds);
        _animalReceiver.OnBroadcastReceived += AnimalReceiverOnOnBroadcastReceived;
        _interactionHandle.OnBeingGrabbed += InteractionHandleOnBeingGrabbed;
        _interactionHandle.OnReleased += InteractionHandleOnReleased;
        _interactionHandle.OnGrabbed += InteractionHandleOnGrabbed;
        _indicators = new GameObject[_indicatorCount];
        for (int i = 0; i < _indicatorCount; i++)
        {
            _indicators[i] = Instantiate(_indicatorPrefab);
            _indicators[i].SetActive(false);
        }
    }

    private void InteractionHandleOnGrabbed(Interactor obj)
    {
        if (_relaxCoroutine != null)
        {
            StopCoroutine(RelaxCoroutine());
            _relaxCoroutine = null;
        }
        for (int i = 0; i < _indicatorCount; i++)
        {
            _indicators[i].SetActive(true);
        }
    }

    private void AnimalReceiverOnOnBroadcastReceived(Animal animal)
    {
        if (!animal.IsFlying)
        {
            _animal = animal;
            _animal.PlaceInSling();
        }
    }

    private void InteractionHandleOnReleased(Interactor obj)
    {
        if (_animal)
        {
            _animal.StartFly(_flyDirection);
            _animal = null;
        }
        _relaxCoroutine = StartCoroutine(RelaxCoroutine());
        for (int i = 0; i < _indicatorCount; i++)
        {
            _indicators[i].SetActive(false);
        }
    }

    private void InteractionHandleOnBeingGrabbed(TransformData[] args)
    {
        Vector3 mean = Vector3.zero;
        foreach (TransformData data in args)
        {
            mean += data.Position;
        }

        mean /= args.Length;
        Vector3 relativePosition = mean - (transform.position + _drawOrigin);
        float magnitude = Mathf.Min(relativePosition.magnitude, _maxMagnitude);
        Vector3 localPosition = relativePosition.normalized * magnitude;
        Vector3 finalPosition = localPosition + _drawOrigin + transform.position;
        _target.transform.position = finalPosition;
        _flyDirection = -(localPosition.normalized * localPosition.sqrMagnitude  * _throwMagnitudeMultiplier);
        if (_animal)
        {
            _animal.transform.position = finalPosition;
        }
        for (int i = 0; i < _indicatorCount; i++)
        {
            float t = (i / (float) _indicatorCount);
            Vector3 down = t * t * Vector3.down;
            Vector3 horizontal = t * _flyDirection;
            Vector3 indicationPosition = finalPosition + down + horizontal;
            _indicators[i].transform.position = indicationPosition;
        }
    }

    private IEnumerator RelaxCoroutine()
    {
        Vector3 endPosition = transform.position + _drawOrigin;
        Vector3 currentPosition;
        do
        {
            currentPosition = _target.transform.position;
            _target.transform.position = Utils.LerpVector(currentPosition, endPosition, Time.deltaTime * 7f);
            yield return null;
        } while (Vector3.Distance(currentPosition, endPosition) > 0.05f);
    }
}
