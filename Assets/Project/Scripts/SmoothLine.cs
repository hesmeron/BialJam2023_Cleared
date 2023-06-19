using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(LineRenderer))]
public class SmoothLine : MonoBehaviour
{
    [SerializeField] 
    private LineRenderer _lineRenderer;

    [SerializeField]
    private Transform[] _lerpPoints;

    [SerializeField] 
    private int positionCount = 12;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandlePositions();
    }

    private void HandlePositions()
    {
        Assert.IsTrue(positionCount >= _lerpPoints.Length);
        Vector3[] points = new Vector3[positionCount];
        for (int i = 0; i < positionCount; i++)
        {
            float raw = (i / (float) positionCount);
            int index = Mathf.FloorToInt(raw * (_lerpPoints.Length-1));

            Vector3 from = _lerpPoints[index].position;
            Vector3 to = _lerpPoints[index+1].position;

            float time = raw - index;
            Vector3 position = Utils.LerpVector(from, to, time);
            points[i] = position;
        }

        _lineRenderer.positionCount = positionCount;
        _lineRenderer.SetPositions(points);
    }
}
