using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingStop : Desitnation
{
    [SerializeField]
    private bool _isOccupied = false;

    public bool IsOccupied
    {
        get => _isOccupied;
        set => _isOccupied = value;
    }

    public override void OnReached(EnemyShip _enemyShip)
    {
        _enemyShip.Land();
    }
}
