using UnityEngine;

public enum EventType
{
    OnDeath,
    OnDamageTake,
    OnMove,
}

public interface IEnemyAction
{
    public void Execute(ProjectileCaster projectileCaster);
}

[System.Serializable]
public class SpawnPJAction : IEnemyAction
{
    [SerializeField] private int _maxPJCount = 2;

    public void Execute(ProjectileCaster caster)
    {
        int PJCount = Random.Range(1, _maxPJCount + 1);
        for (int i = 0;  i < PJCount; i++)
            caster.Shoot();
    }
}

