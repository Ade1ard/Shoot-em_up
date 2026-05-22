using UnityEngine;

public enum EventType
{
    OnDeath,
    OnDamageTake,
    OnMove,
}

public interface IEnemyAction
{
    public void Execute();
}
