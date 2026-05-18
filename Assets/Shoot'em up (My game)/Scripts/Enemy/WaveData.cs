using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemyGroup
    {
        [Header("EnemyPrefab")]
        public Enemy _enemyPrefab;

        [Header("Counts")]
        public int _minEnemyCount;
        public int _maxEnemyCount;

        [Header("Settings")]
        public bool _spawnTogether;
        public float _delayBetweenEnemies;
        public float _delayAfterGroup;
    }

    [Header("Pattern")]
    [SerializeReference, SubclassSelector]
    public ISpawnFormation _spawnPattern;

    [Header("Settings")]
    public int _difficultyLevel = 1;

    [Header("Wave")]
    public List<EnemyGroup> _enemyGroups = new List<EnemyGroup>();

    [Header("Timing")]
    public float _waveStartDelay = 2f;

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
