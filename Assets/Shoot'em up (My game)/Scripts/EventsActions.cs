using UnityEngine;
using System.Collections;

public enum EnemyEventType
{
    OnDeath,
    OnDamageTake,
    OnMove,
    OnHit,
}

public enum PlayerEventType
{
    OnDamageTake,
    OnMove,
    OnHeal,
    OnHit,
    OnSelectOnce,
    OnEnemyDeath,
}

public interface IAction
{
    public void Execute(ActionContext context);
}

[System.Serializable]
public class ActionWrapper
{
    [SerializeReference, SubclassSelector]
    public IAction Action;

    public RepeatMode RepeatMode;
    public int RepeatCount = 1;
    public float Interval = 0.5f;
}

public enum RepeatMode
{
    Once,
    Count,
    Infinite,
}

public class ActionRunner
{
    private ActionWrapper _wrapper;
    private ActionContext _context;
    private MonoBehaviour _owner;
    private Coroutine _coroutine;
    private int _executedCount;

    public ActionRunner(ActionWrapper wrapper, ActionContext context, MonoBehaviour owner)
    {
        _wrapper = wrapper;
        _context = context;
        _owner = owner;
    }

    public void Start()
    {
        _executedCount = 0;

        switch (_wrapper.RepeatMode)
        {
            case RepeatMode.Once:
                _wrapper.Action.Execute(_context);
                break;

            case RepeatMode.Count:
            case RepeatMode.Infinite:
                _coroutine = _owner.StartCoroutine(RunRepeating());
                break;
        }
    }

    public void Stop()
    {
        if (_coroutine != null)
            _owner.StopCoroutine(_coroutine);
    }

    private IEnumerator RunRepeating()
    {
        while (true)
        {
            _wrapper.Action.Execute(_context);
            _executedCount++;

            if (_wrapper.RepeatMode == RepeatMode.Count && _executedCount >= _wrapper.RepeatCount)
                break;

            yield return new WaitForSeconds(_wrapper.Interval);
        }
    }
}

public class ActionContext
{
    public ProjectileCaster Caster;
    public Health Health;
    public Player Player;
    public Vector3? ExecutePos;

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
public class ChangeHPAction : IAction
{
    [SerializeField] private int _healAmount = 1;

    public void Execute(ActionContext context)
    {
        if (context.Health == null) return;

        context.Health.ChangeHP(_healAmount);
    }
}

[System.Serializable]
public class CustomShoot : IAction
{
    public CustomShootContext _context;

    public void Execute(ActionContext context)
    {
        if (context.Caster == null) return;

        context.Caster.CustomShoot(_context, context.ExecutePos);
    }
}

[System.Serializable]
public class CustomShootContext
{
    [Header("Prefab")]
    public ProjectileCont _projectilePrefab;

    [Header("SpawnPattern")]
    [SerializeReference, SubclassSelector]
    public ISpawnFormation _spawnPattern;

    [Header("Parameters")]
    public float _PJDamage;
    public float _PJLifeTime;
    public int _PJCount;
    public bool _disposable;

    [Header("Effects")]
    public ParticleSystem _vFX;
    public AudioClip _sound;
}