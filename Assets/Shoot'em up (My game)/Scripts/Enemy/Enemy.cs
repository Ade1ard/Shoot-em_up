using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Enemy : Health, IHitHandler
{
    [Header("Parameters")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int xpReward = 10;
    [FormerlySerializedAs("PRJDamage")] [SerializeField] private float prjDamage = 10;
    [SerializeField] private float shootDelay = 2;
    [SerializeField] private int projectileCount = 1;

    [Header("Events")]
    [SerializeField] private List<EventSetup> eventSetups = new List<EventSetup>();
    private ActionContext _context;
    private Dictionary<EnemyEventType, List<ActionRunner>> _activeRunners = new Dictionary<EnemyEventType, List<ActionRunner>>();

    [System.Serializable]
    public class EventSetup
    {
        public EnemyEventType eventType;
        public List<ActionWrapper> actions = new List<ActionWrapper>();
    }

    private Vector3 _previousPositon;
    private float _currentSpeed;
    private float _onMoveTriggerDelay = 1;
    private float _onMoveLastTrigger;

    private bool _canDamage = true;

    private ProjectileCaster _projectileCaster;

    public event Action<Enemy, int> OnDeath;

    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<Animator>().SetFloat("StartOffset", UnityEngine.Random.Range(0f, 1f));
    }

    public void Initialize(float multiplier = 0)
    {
        _projectileCaster = GetComponent<ProjectileCaster>();

        if (multiplier != 0)
        {
            maxHealth *= multiplier;
        }

        base.InitHP(maxHealth);

        if (_projectileCaster != null)
        {
            _projectileCaster.TakeStats(prjDamage, shootDelay, projectileCount);
            _projectileCaster.IsShooting(true);
        }

        ProjectileCont PJCont = GetComponent<ProjectileCont>();
        if (PJCont != null)
        {
            PJCont.Initialize(prjDamage);
            PJCont.ItEnemy();
        }

        _context = ActionContext.FromEnemy(this);
    }

    public override void DealDamage(float damage, Vector3 closestPoint = default)
    {
        base.DealDamage(damage, closestPoint);
        TriggerEvent(EnemyEventType.OnDamageTake);
    }

    public override bool CanDamage() { return _canDamage; }

    protected override void Death()
    {
        _canDamage = false;
        base.UIVisible(false);
        DOTween.Kill(transform);
        base.Death();
        TriggerEvent(EnemyEventType.OnDeath);
        OnDeath?.Invoke(this, xpReward);
        Destroy(gameObject, 0.1f);
    }

    private void Update()
    {
        _currentSpeed = Vector3.Distance(transform.position, _previousPositon) / Time.deltaTime;
        _previousPositon = transform.position;

        if (_currentSpeed > 2 && Time.time - _onMoveLastTrigger >= _onMoveTriggerDelay)
        {
            TriggerEvent(EnemyEventType.OnMove);
            _onMoveLastTrigger = Time.time;
        }
    }

    void IHitHandler.OnHit(Vector3 pos)
    {
        TriggerEvent(EnemyEventType.OnHit);
    }

    public void TriggerEvent(EnemyEventType eventType)
    {
        var setup = eventSetups.Find(e => e.eventType == eventType);
        if (setup == null) return;

        if (_activeRunners.TryGetValue(eventType, out var oldRunners))
        {
            foreach (var r in oldRunners) r.Stop();
        }

        var runners = new List<ActionRunner>();
        foreach (var wrapper in setup.actions)
        {
            var runner = new ActionRunner(wrapper, _context, this);
            runner.Start();
            runners.Add(runner);
        }

        _activeRunners[eventType] = runners;
    }
}