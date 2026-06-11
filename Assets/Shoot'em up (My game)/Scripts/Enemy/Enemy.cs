using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Enemy : Health, IHitHandler
{
    [Header("Parameters")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int xpReward = 10;
    [SerializeField] private float PRJDamage = 10;
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

    private ProjectileCaster _projectileCaster;
    private SpriteRenderer _sprite;

    public event Action<Enemy, int> OnDeath;

    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<Animator>().SetFloat("StartOffset", UnityEngine.Random.Range(0f, 1f));
    }

    public void Initialize(float multiplier = default)
    {
        _projectileCaster = GetComponent<ProjectileCaster>();
        _sprite = GetComponentInChildren<SpriteRenderer>();

        if (multiplier != 0)
        {
            maxHealth *= multiplier;
            EnemyMutate(multiplier);
        }

        base.InitHP(maxHealth);

        if (_projectileCaster != null)
        {
            _projectileCaster.TakeStats(PRJDamage, shootDelay, projectileCount);
            _projectileCaster.IsShooting(true);
        }

        ProjectileCont PJCont = GetComponent<ProjectileCont>();
        if (PJCont != null)
        {
            PJCont.Initialize(PRJDamage);
            PJCont.ItEnemy();
        }

        _context = ActionContext.FromEnemy(this);
    }

    private void EnemyMutate(float difficulty)
    {
        if (difficulty < 2) return;

        bool mutated = false;

        if (TryMutate(difficulty))
        {
            mutated = true;
            shootDelay = UnityEngine.Random.Range(1, shootDelay + 1);
            xpReward += 20;
        }

        if (TryMutate(difficulty))
        {
            mutated = true;
            projectileCount = UnityEngine.Random.Range(projectileCount, projectileCount + 3);
            xpReward += 20;
        }

        if (TryMutate(difficulty))
        {
            mutated = true;
            maxHealth *= difficulty;
            xpReward += 20;
        }

        if (mutated)
        {
            if (_projectileCaster != null)
                _projectileCaster.TakeStats(PRJDamage, shootDelay, projectileCount);
            if (_sprite != null)
                _sprite.color = Color.HSVToRGB(UnityEngine.Random.Range(0, 1f), 0.47f, 1);
        }
    }

    private bool TryMutate(float multiplier) { return UnityEngine.Random.Range(0, 100) < 5 * multiplier; }

    public override void DealDamage(float damage, Vector3 closestPoint = default)
    {
        base.DealDamage(damage, closestPoint);
        TriggerEvent(EnemyEventType.OnDamageTake);
    }

    protected override void Death()
    {
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