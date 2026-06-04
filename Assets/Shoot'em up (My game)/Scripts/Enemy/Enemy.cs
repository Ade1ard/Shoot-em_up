using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Enemy : Health
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

    [System.Serializable]
    public class EventSetup
    {
        public EnemyEventType eventType;
        [SerializeReference, SubclassSelector]
        public List<IAction> actions = new List<IAction>();
    }

    private Vector3 _previousPositon;
    private float _currentSpeed;
    private float _onMoveTriggerDelay = 1;
    private float _onMoveLastTrigger;

    private ProjectileCaster _projectileCaster;

    public event Action<Enemy, int> OnDeath;

    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<Animator>().SetFloat("StartOffset", UnityEngine.Random.Range(0f, 1f));
    }

    public void Initialize(float multiplier = default)
    {
        if (multiplier != 0)
            maxHealth *= multiplier;

        base.InitHP(maxHealth);

        _projectileCaster = GetComponent<ProjectileCaster>();
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

    private void TriggerEvent(EnemyEventType eventType)
    {
        var setup = eventSetups.Find(e => e.eventType == eventType);
        if (setup != null)
        {
            foreach (var action in setup.actions)
            {
                action.Execute(_context);
            }
        }
    }
}