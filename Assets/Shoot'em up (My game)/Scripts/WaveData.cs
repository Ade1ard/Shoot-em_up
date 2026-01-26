using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemyGroup
    {
        [Header("EnemyPrefab")]
        public GameObject _enemyPrefab;

        [Header("Counts")]
        public int _minEnemyCount;
        public int _maxEnemyCount;

        [Header("Settings")]
        public bool _spawnTogether;
        public float _delayBetweenEnemies;
        public float _delayAfterGroup;
    }

    [System.Serializable]
    public class SpawnPattern
    {
        public enum PatternType
        {
            Random,
            Circle,     
            Line,
            VFormation,
            Grid,
        }

        [Header("Pattern")]
        public PatternType patternType = PatternType.Random;

        [Header("Pattern Settings")]
        public float _radius = 5f; // Circle
        public Vector2 _spacing = new Vector2(1.5f, 1.5f); // Grid
        public int _columns = 3; // Grid/Line
    }

    [Header("Settings")]
    public string _waveName = "New Wave";
    public int _difficultyLevel = 1;

    [Header("Wave")]
    public List<EnemyGroup> _enemyGroups = new List<EnemyGroup>();

    [Header("Pattern")]
    public SpawnPattern _spawnPattern = new SpawnPattern();

    [Header("Timing")]
    public float _waveStartDelay = 2f;
    public float _timeBetweenGroups = 3f;

    [Header("Rewards")]
    public int _baseXP = 100;

    public int GetRandomCountForGroup(int groupIndex)
    {
        if (groupIndex >= _enemyGroups.Count) return 0;
        EnemyGroup group = _enemyGroups[groupIndex];
        return Random.Range(group._minEnemyCount, group._maxEnemyCount + 1);
    }

    public int GetTotalEnemyCount()
    {
        int total = 0;
        foreach (var group in _enemyGroups)
        {
            total += (group._minEnemyCount + group._maxEnemyCount) / 2;
        }
        return total;
    }
}
