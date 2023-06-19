using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector3 LerpVector(Vector3 from, Vector3 to, float time)
    {
        float x = Mathf.Lerp(from.x, to.x, time);
        float y = Mathf.Lerp(from.y, to.y, time);
        float z = Mathf.Lerp(from.z, to.z, time);
        return new Vector3(x, y, z);
    }
}
