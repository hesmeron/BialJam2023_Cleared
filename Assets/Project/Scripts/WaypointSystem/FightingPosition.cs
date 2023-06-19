using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingPosition : Desitnation
{
    [SerializeField] 
    private GameManager _gameManager;
    [SerializeField]
    private LandingStop[] _landingStops;
    public override void OnReached(EnemyShip _enemyShip)
    {
        List<LandingStop> availableStops = new List<LandingStop>();
        foreach (LandingStop landingStop in _landingStops)
        {
            if (!landingStop.IsOccupied)
            {
                availableStops.Add(landingStop);
            }
        }

        if (availableStops.Count > 1)
        {
            LandingStop shipStop = availableStops[Random.Range(0, availableStops.Count)];
            shipStop.IsOccupied = true;
            _enemyShip.SetLanding(shipStop);
        }
        else
        {
            _gameManager.Lose();
        }

    }
}
