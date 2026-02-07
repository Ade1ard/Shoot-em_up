using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn points")]
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    [Header("Waves")]
    [SerializeField] private List<WaveData> _availableWaves = new List<WaveData>();
    [SerializeField] private float _timeBetweenWaves = 5f;

    [Header("Difficulty")]
    [SerializeField] private float _currentDifficulty = 1f;
    [SerializeField] private float _difficultyIncreasePerWave = 0.1f;

    private int _currentWaveNumber = 1;
    private List<GameObject> _activeEnemiesCount = new List<GameObject>();
    private WaveData _currentWaveData;
    private Vector3 _basePosition;

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            _currentWaveData = SelectNextWave();

            Debug.Log($"Wave begin {_currentWaveNumber}: {_currentWaveData._waveName}: Wave difficulty{_currentWaveData._difficultyLevel}: Current difficulty {_currentDifficulty}");

            yield return new WaitForSeconds(_currentWaveData._waveStartDelay);

            yield return StartCoroutine(SpawnWave(_currentWaveData));

            yield return new WaitForSeconds(_timeBetweenWaves);
        }
    }

    WaveData SelectNextWave()
    {
        List<WaveData> possibleWaves = new List<WaveData>();

        foreach (WaveData wave in _availableWaves)
        {
            if (wave._difficultyLevel <= _currentDifficulty)
                possibleWaves.Add(wave);
        }

        return possibleWaves[Random.Range(0, possibleWaves.Count)];
    }

    IEnumerator SpawnWave(WaveData currentWave)
    {
        for (int groupIndex = 0; groupIndex < currentWave._enemyGroups.Count; groupIndex++)
        {
            WaveData.EnemyGroup currentGroup = currentWave._enemyGroups[groupIndex];

            int enemyCount = currentWave.GetRandomCountForGroup(groupIndex);

            _basePosition = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnSingleEnemy(currentGroup._enemyPrefab, CalculateSpawnPosition(currentWave._spawnPattern, i, enemyCount, currentGroup._spawnTogether));
                if (!currentGroup._spawnTogether)
                    yield return new WaitForSeconds(currentGroup._delayBetweenEnemies);
            }

            yield return new WaitForSeconds(currentGroup._delayAfterGroup);

            if (groupIndex < currentWave._enemyGroups.Count - 1)
            {
                yield return new WaitForSeconds(currentWave._timeBetweenGroups);
            }
        }
        _currentWaveNumber++;
        _currentDifficulty += _difficultyIncreasePerWave;
    }

    void SpawnSingleEnemy(GameObject enemyPrefab, Vector3 spawnPosition)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemyPrefab is null");
            return;
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<ObjectMovement>().StartMove(spawnPosition);

        //EnemyController enemyController = enemy.GetComponent<EnemyController>();
        //if (enemyController != null)
        //{
        //    enemyController.Initialize(difficultyMultiplier);
        //    enemyController.OnDeath += HandleEnemyDeath;

        //    if (playerTransform != null)
        //    {
        //        enemyController.SetTarget();
        //    }
        //}

        _activeEnemiesCount.Add(enemy);
    }

    Vector3 CalculateSpawnPosition(WaveData.SpawnPattern pattern, int index, int enemyCount, bool spawnTogether)
    {

        if (_spawnPoints.Count > 0)
        {
            if (!spawnTogether)
                _basePosition = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
        }
        else
        {
            _basePosition = transform.position;
        }

        Vector3 offset = Vector3.zero;

        switch (pattern.patternType)
        {
            case WaveData.SpawnPattern.PatternType.Random:
                offset = new Vector3(
                    Random.Range(-pattern._radius, pattern._radius),
                    Random.Range(-pattern._radius / 2, pattern._radius / 2),
                    0
                );
                break;

            case WaveData.SpawnPattern.PatternType.Circle:
                float angle = (index / (float)enemyCount) * Mathf.PI * 2;
                offset = new Vector3(
                    Mathf.Cos(angle) * pattern._radius,
                    Mathf.Sin(angle) * pattern._radius,
                    0
                );
                break;

            case WaveData.SpawnPattern.PatternType.Line:
                float t = index / (float)Mathf.Max(1, enemyCount - 1);
                offset = new Vector3(
                    Mathf.Lerp(-pattern._radius, pattern._radius, t),
                    0,
                    0
                );
                break;

            case WaveData.SpawnPattern.PatternType.VFormation:
                float centerIndex = (enemyCount - 1) / 2f;
                float distanceFromCenter = Mathf.Abs(index - centerIndex);
                offset = new Vector3(
                    (index - centerIndex) * pattern._spacing.x,
                    -distanceFromCenter * pattern._spacing.y,
                    0
                );
                break;

            case WaveData.SpawnPattern.PatternType.Grid:
                int row = index / pattern._columns;
                int col = index % pattern._columns;
                offset = new Vector3(
                    (col - (pattern._columns - 1) / 2f) * pattern._spacing.x,
                    -row * pattern._spacing.y,
                    0
                );
                break;
        };

        return _basePosition + offset;
    }

    void HandleEnemyDeath(GameObject enemy, int xpValue)
    {
        if (_activeEnemiesCount.Contains(enemy))
        {
            _activeEnemiesCount.Remove(enemy);

            //PlayerStats.Instance?.AddXP(xpValue);
            //PlayerStats.Instance?.AddCoins(currentWave.coinReward);
        }
    }

    void OnDestroy()
    {
        foreach (var enemy in _activeEnemiesCount)
        {
            if (enemy != null)
            {
                var enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    //enemyController.OnDeath -= HandleEnemyDeath;
                }
            }
        }
    }
}