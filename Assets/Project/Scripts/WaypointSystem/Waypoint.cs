
using UnityEngine;

public class Waypoint :  Desitnation
{
    [SerializeField] 
    private Desitnation[] _possibleDesinations;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        base.OnDrawGizmos();
        foreach (Desitnation possibleDesination in _possibleDesinations)
        {
            Gizmos.DrawLine(transform.position, possibleDesination.transform.position);
        }
    }

    public override void OnReached(EnemyShip _enemyShip)
    {
        if ( _possibleDesinations.Length > 0)
        {
            _enemyShip.SetNewDestination(_possibleDesinations[Random.Range(0, _possibleDesinations.Length)]);
        }
    }
}
