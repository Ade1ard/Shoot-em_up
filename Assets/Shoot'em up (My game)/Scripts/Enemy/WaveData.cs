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
    public float _difficultyLevel = 1;
    public bool _isAvaible = true;

    [Header("Wave")]
    public List<EnemyGroup> _enemyGroups = new List<EnemyGroup>();

    [Header("Timing")]
    public float _waveStartDelay = 2f;

    public int GetRandomCountForGroup(int groupIndex, int extraEnemies = 0)
    {
        if (groupIndex >= _enemyGroups.Count) return 0;
        EnemyGroup group = _enemyGroups[groupIndex];
        return Random.Range(group._minEnemyCount + extraEnemies, group._maxEnemyCount + extraEnemies + 1);
    }
}