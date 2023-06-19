using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemySpawnPoint[] _enemySpawnPoints;
    [SerializeField] 
    private GameManager _gameManager;
    [SerializeField]
    private EnemyShip _enemyShipPrefab;
    [SerializeField]
    private float _timeToPass = 4f;
    [SerializeField]
    private Vector2 _range = new Vector2(2f, 7f);
    [SerializeField]
    private float _timePassed = 0f;

    [SerializeField]
    private int _shipsLeft = 44;

    private void Awake()
    {
        _gameManager.OnGameEnded += GameManagerOnOnGameEnded;
        _gameManager.OnGameStarted += GameManagerOnOnGameStarted;
    }

    private void GameManagerOnOnGameEnded()
    {
        Destroy(this);
    }
    private void GameManagerOnOnGameStarted()
    {
        StartCoroutine(SpawShips());
    }

    // Update is called once per frame
   IEnumerator SpawShips()
    {
        while (_shipsLeft > 0)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed > _timeToPass)
            {
                _timePassed = 0f;
                _timeToPass = Random.Range(_range.x, _range.y);
                Spawn();
            }
            yield return null;
        }
    }


    private void Spawn()
    {
        _shipsLeft--;
        int index = Random.Range(0, _enemySpawnPoints.Length);
        EnemySpawnPoint enemySpawnPoint = _enemySpawnPoints[index];
        EnemyShip newShip = Instantiate(_enemyShipPrefab);
        newShip.Initialize(enemySpawnPoint);
        if (_shipsLeft == 0)
        {
            _gameManager.Win();
        }
    }
}
