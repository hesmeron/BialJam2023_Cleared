using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Desitnation : MonoBehaviour
{
    [SerializeField] 
    private float _reachRadius;

    protected void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, _reachRadius);
    }

    public bool TryReach(float magnitude, EnemyShip enemyShip, bool ignoreCallback = false)
    {
        if (magnitude <= _reachRadius)
        {
            if (!ignoreCallback)
            {
                OnReached(enemyShip);
            }
            return true;
        }

        return false;
    }

    public abstract void OnReached(EnemyShip _enemyShip);
}
