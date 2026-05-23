using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;

public class EnemySpawner : MonoBehaviour, IInitializable
{
    [Header("Spawn points")]
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    [Header("Param")]
    [SerializeField] private int _maxWavesSimultaneosly = 3;
    [SerializeField] private int _maxWavesOffset = 1;
    [SerializeField] private float _timeBetweenWaves = 5f;

    [Header("Difficulty")]
    [SerializeField] private float _currentDifficulty = 1f;
    [SerializeField] private float _difficultyIncreasePerWave = 0.1f;

    private List<WaveData> _availableWaves = new List<WaveData>();
    private int _currentWaveNumber = 0;
    private List<Enemy> _activeEnemies = new List<Enemy>();
    private List<WaveData> _currentWaves = new List<WaveData>();
    private Vector3 _basePosition;
    private Player _player;
    private UIView _UIView;

    private Action<bool> _UIVisible;

    public void Init()
    {
        _player = G.Get<Player>();
        _UIView = G.Get<UIView>();
        _UIView.ShowCurrentWave(1);

        LoadWaves();
    }

    private void LoadWaves()
    {
        var waves = Resources.LoadAll<WaveData>("Waves").ToList();
        foreach (var wave in waves)
            if (wave._isAvaible)
                _availableWaves.Add(wave);
    }

    public void StartSpawning()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            int maxWaves = _maxWavesSimultaneosly + UnityEngine.Random.Range(-_maxWavesOffset, _maxWavesOffset+1);

            _currentWaves = SelectNextWaves(maxWaves);

            _currentWaveNumber++;
            Debug.Log($"Wave begin {_currentWaveNumber} : Current difficulty {_currentDifficulty} : WavesCount {_currentWaves.Count}");
            _UIView.ShowCurrentWave(_currentWaveNumber);
            _currentDifficulty += _difficultyIncreasePerWave;

            foreach (var wave in _currentWaves)
            {
                yield return new WaitForSeconds(wave._waveStartDelay);

                yield return StartCoroutine(SpawnWave(wave));

                yield return new WaitForSeconds(_timeBetweenWaves);
            }

            yield return new WaitUntil(() => _activeEnemies.Count == 0);
            _currentWaves.Clear();
        }
    }

    List<WaveData> SelectNextWaves(int maxWaves)
    {
        List<WaveData> possibleWaves = new List<WaveData>();

        foreach (WaveData wave in _availableWaves)
        {
            if (wave._difficultyLevel <= _currentDifficulty)
                possibleWaves.Add(wave);
        }

        List<WaveData> waves = new List<WaveData>();
        if (maxWaves > possibleWaves.Count)
            maxWaves = possibleWaves.Count;
        while (waves.Count < maxWaves)
        {
            var wave = possibleWaves[UnityEngine.Random.Range(0, possibleWaves.Count)];
            if (!waves.Contains(wave))
                waves.Add(wave);
        }

        return waves;
    }

    IEnumerator SpawnWave(WaveData currentWave)
    {
        for (int groupIndex = 0; groupIndex < currentWave._enemyGroups.Count; groupIndex++)
        {
            WaveData.EnemyGroup currentGroup = currentWave._enemyGroups[groupIndex];

            int enemyCount = currentWave.GetRandomCountForGroup(groupIndex, (int)Mathf.Floor(_currentDifficulty - 1));

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
        _UIVisible += enemy.UIVisible;
        enemy.OnDeath += HandleEnemyDeath;

        _activeEnemies.Add(enemy);
    }

    Vector3 CalculateSpawnPosition(ISpawnFormation spawnPattern, int index, int enemyCount, bool spawnTogether)
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

        return spawnPattern.CalculateSpawnPosition(_basePosition, index, enemyCount);
    }

    public void AllEnemiesUIVisible(bool visible)
    {
        _UIVisible?.Invoke(visible);
    }

    void HandleEnemyDeath(Enemy enemy, int xpValue)
    {
        if (_activeEnemies.Contains(enemy))
        {
            enemy.OnDeath -= HandleEnemyDeath;
            _UIVisible -= enemy.UIVisible;
            _activeEnemies.Remove(enemy);

            _player.AddXP((int)(xpValue * _currentDifficulty), _currentDifficulty);
        }
    }

    public void ClearAllEnemies()
    {
        foreach (Enemy enemy in _activeEnemies)
        {
            enemy.OnDeath -= HandleEnemyDeath;
            _UIVisible -= enemy.UIVisible;
            DOTween.Kill(enemy.transform);
            Destroy(enemy.gameObject);
        }
        _activeEnemies.Clear();
    }

    public void SetBasicDifficulty()
    {
        _currentDifficulty = 1;
        _currentWaveNumber = 0;
    }

    void OnDestroy()
    {
        foreach (var enemy in _activeEnemies)
            if (enemy != null)
                enemy.OnDeath -= HandleEnemyDeath;
    }
}