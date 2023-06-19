using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    [Range(0, 100)]
    private float _timeScale = 1f;

    void UpdateTimeScale()
    {
        Time.timeScale = _timeScale;
    }
    
}
