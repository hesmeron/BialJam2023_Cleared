using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : Desitnation
{
    [SerializeField]
    private Waypoint _next;

    public Waypoint Next => _next;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawLine(transform.position, _next.transform.position);
    }

    public override void OnReached(EnemyShip _enemyShip)
    {
        _enemyShip.gameObject.SetActive(false);
    }

    private void Spawn()
    {
        
    }
}
