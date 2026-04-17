using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour, IInitializable
{
    [Header("Spawn points")]
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    [Header("Waves")]
    [SerializeField] private List<WaveData> _availableWaves = new List<WaveData>();
    [SerializeField] private float _timeBetweenWaves = 5f;

    [Header("Difficulty")]
    [SerializeField] private float _currentDifficulty = 1f;
    [SerializeField] private float _difficultyIncreasePerWave = 0.1f;

    private int _currentWaveNumber = 0;
    private List<Enemy> _activeEnemies = new List<Enemy>();
    private WaveData _currentWaveData;
    private Vector3 _basePosition;
    private Player _player;
    private UIView _UIView;

    private Action _hideUI;

    public void Init()
    {
        _player = G.Get<Player>();
        _UIView = G.Get<UIView>();
        _UIView.ShowCurrentWave(1);
    }

    public void StartSpawning()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            _currentWaveData = SelectNextWave();

            _currentWaveNumber++;
            Debug.Log($"Wave begin {_currentWaveNumber}: {_currentWaveData._waveName}: Wave difficulty{_currentWaveData._difficultyLevel}: Current difficulty {_currentDifficulty}");
            _UIView.ShowCurrentWave(_currentWaveNumber);
            _currentDifficulty += _difficultyIncreasePerWave;

            yield return new WaitForSeconds(_currentWaveData._waveStartDelay);

            yield return StartCoroutine(SpawnWave(_currentWaveData));

            yield return new WaitUntil(() => _activeEnemies.Count == 0);

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

        return possibleWaves[UnityEngine.Random.Range(0, possibleWaves.Count)];
    }

    IEnumerator SpawnWave(WaveData currentWave)
    {
        for (int groupIndex = 0; groupIndex < currentWave._enemyGroups.Count; groupIndex++)
        {
            WaveData.EnemyGroup currentGroup = currentWave._enemyGroups[groupIndex];

            int enemyCount = currentWave.GetRandomCountForGroup(groupIndex);

            _basePosition = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)].position;

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnSingleEnemy(currentGroup._enemyPrefab, CalculateSpawnPosition(currentWave._spawnPattern, i, enemyCount, currentGroup._spawnTogether));
                if (!currentGroup._spawnTogether)
                    yield return new WaitForSeconds(currentGroup._delayBetweenEnemies);
            }

            yield return new WaitForSeconds(currentGroup._delayAfterGroup);
        }
    }

    void SpawnSingleEnemy(Enemy enemyPrefab, Vector3 spawnPosition)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemyPrefab is null");
            return;
        }

        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.gameObject.GetComponent<ObjectMovement>().StartMove(spawnPosition);

        enemy.Initialize(_currentDifficulty);
        _hideUI += enemy.HideUI;
        enemy.OnDeath += HandleEnemyDeath;

        _activeEnemies.Add(enemy);
    }

    Vector3 CalculateSpawnPosition(WaveData.SpawnPattern pattern, int index, int enemyCount, bool spawnTogether)
    {

        if (_spawnPoints.Count > 0)
        {
            if (!spawnTogether)
                _basePosition = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)].position;
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
                    UnityEngine.Random.Range(-pattern._radius, pattern._radius),
                    UnityEngine.Random.Range(-pattern._radius / 2, pattern._radius / 2),
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

    public void HideAllEnemiesUI()
    {
        _hideUI?.Invoke();
    }

    void HandleEnemyDeath(Enemy enemy, int xpValue)
    {
        if (_activeEnemies.Contains(enemy))
        {
            enemy.OnDeath -= HandleEnemyDeath;
            _hideUI -= enemy.HideUI;
            _activeEnemies.Remove(enemy);

            _player.AddXP((int)(xpValue * _currentDifficulty), _currentDifficulty);
        }
    }

    void OnDestroy()
    {
        foreach (var enemy in _activeEnemies)
            if (enemy != null)
                enemy.OnDeath -= HandleEnemyDeath;
    }
}