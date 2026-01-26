using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor.Overlays;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn points")]
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    [Header("Waves")]
    [SerializeField] private List<WaveData> _availableWaves = new List<WaveData>();
    [SerializeField] private float _timeBetweenWaves = 5f;

    [Header("Difficulty")]
    [SerializeField] private float _currentDifficulty = 1f;
    [SerializeField] private float _difficultyMultiplier = 1f;
    [SerializeField] private float _difficultyIncreasePerWave = 0.1f;

    [Header("Random")]
    [Range(0f, 1f)] public float waveSelectionRandomness = 0.3f;

    private int _currentWaveNumber = 1;
    private bool _isSpawning;
    private List<GameObject> _activeEnemiesCount = new List<GameObject>();
    private WaveData _currentWaveData;

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            _currentWaveData = SelectNextWave();

            Debug.Log($"Waave begin {_currentWaveNumber}: {_currentWaveData._waveName}: {_currentWaveData._difficultyLevel}");

            yield return new WaitForSeconds(_currentWaveData._waveStartDelay);

            _isSpawning = true;
            yield return StartCoroutine(SpawnWave(_currentWaveData));
            _isSpawning = false;
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

            if (currentGroup._spawnTogether)
            {
                SpawnGroupTogether(currentGroup._enemyPrefab, enemyCount, currentWave._spawnPattern);
            }
            else
            {
                
            }

            yield return new WaitForSeconds(currentGroup._delayAfterGroup);

            if (groupIndex < currentWave._enemyGroups.Count - 1)
            {
                yield return new WaitForSeconds(currentWave._timeBetweenGroups);
            }
        }
    }

    void SpawnGroupTogether(GameObject enemyPrefab, int Count, WaveData.SpawnPattern spawnPattern)
    {

    }

    void SpawnSingleEnemy(GameObject enemyPrefab, WaveData.SpawnPattern spawnPattern)
    {

    }
}
