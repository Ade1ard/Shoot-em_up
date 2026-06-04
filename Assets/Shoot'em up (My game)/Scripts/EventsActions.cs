using UnityEngine;

public enum EnemyEventType
{
    OnDeath,
    OnDamageTake,
    OnMove,
}

public enum PlayerEventType
{
    OnDamageTake,
    OnMove,
    OnHeal,
}

public interface IAction
{
    public void Execute(ActionContext context);
}

public class ActionContext
{
    public ProjectileCaster Caster;
    public Health Health;
    public Player Player;

    public static ActionContext FromEnemy(Enemy enemy)
    {
        return new ActionContext
        {
            Caster = enemy.GetComponent<ProjectileCaster>(),
            Health = enemy,
            Player = null
        };
    }

    public static ActionContext FromPlayer(Player player)
    {
        return new ActionContext
        {
            Caster = player.GetComponent<ProjectileCaster>(),
            Health = player,
            Player = player
        };
    }
}

[System.Serializable]
public class SpawnPJAction : IAction
{
    [SerializeField] private int _maxPJCount = 2;

    public void Execute(ActionContext context)
    {
        if (context.Caster == null) return;

        int PJCount = Random.Range(1, _maxPJCount + 1);
        for (int i = 0;  i < PJCount; i++)
            context.Caster.Shoot();
    }
}

[System.Serializable]
public class Heal : IAction
{
    [SerializeField] private int _healAmount = 1;

    public void Execute(ActionContext context)
    {
        if (context.Health == null) return;

        context.Health.HealHP(_healAmount);
    }
}