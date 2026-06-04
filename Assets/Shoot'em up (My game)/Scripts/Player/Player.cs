using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ProjectileCaster))]
public class Player : Health, IInitializable, IHitHandler
{
    [Header("Canvas")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private float _canvasFollowSpeed = 35;

    [Header("Animation")]
    [SerializeField] private float _animDuration = 1;
    [SerializeField] private AnimationCurve _fadeCurve;
    [SerializeField] private AnimationCurve _appearCurve;

    [Header("HitAnim")]
    [SerializeField] private Color _hitColor;
    [SerializeField] private float _noHitDuration = 1;

    [Header("Events")]
    [SerializeField] private List<EventSetup> _eventSetups = new List<EventSetup>();
    private ActionContext _context;

    [System.Serializable]
    public class EventSetup
    {
        public PlayerEventType eventType;
        [SerializeReference, SubclassSelector]
        public List<IAction> actions = new List<IAction>();
    }

    private PlayerData _playerData;

    private PlayerRuntimeStats _stats;
    public PlayerRuntimeStats Stats => _stats;

    private PlayerModifiers _modifiers;
    public PlayerModifiers Modifiers => _modifiers;

    private Dictionary<Type, PlayerWeaponConfig> _weaponConfigs = new Dictionary<Type, PlayerWeaponConfig>();
    private PlayerWeaponConfig _currentWeaponConfig;
    public PlayerWeaponConfig CurrentWeaponConfig => _currentWeaponConfig;

    private Dictionary<Type, PlayerPJMoveConfig> _PJMoveConfigs = new Dictionary<Type, PlayerPJMoveConfig>();
    private PlayerPJMoveConfig _currentPJMoveConfig;
    public PlayerPJMoveConfig CurrentPJMoveConfig => _currentPJMoveConfig;

    private ProjectileCaster _playerPRJCaster;
    private SpriteRenderer _sprite;
    private CameraShake _cameraShake;

    private float _lastHitTime;

    public Action OnLevelUp;
    public Action OnPlayerDied;
    public Action<int, int> OnXPChanged;
    public Action<int, int> OnScoreChanged;

    private Vector3 v = Vector3.zero;
    private Vector2 _startPosition;
    private Vector3 _originalScale;

    private Vector3 _previousPositon;
    private float _currentSpeed;
    private float _onMoveTriggerDelay = 1;
    private float _onMoveLastTrigger;

    public void Init()
    {
        _modifiers = new PlayerModifiers();
        _stats = new PlayerRuntimeStats();
        _playerPRJCaster = GetComponent<ProjectileCaster>();
        _sprite = GetComponent<SpriteRenderer>();
        _cameraShake = G.Get<CameraShake>();

        _playerData = Resources.Load<PlayerData>("Player/PlayerData");
        var weaponConfigs = Resources.LoadAll<PlayerWeaponConfig>("Player").ToList();
        foreach (var config in weaponConfigs)
        {
            var type = config.SpawnPattern.GetType();
            _weaponConfigs[type] = config;
        }

        var PJMoveConfigs = Resources.LoadAll<PlayerPJMoveConfig>("Player").ToList();
        foreach (var config in PJMoveConfigs)
        {
            var type = config.MovementType.GetType();
            _PJMoveConfigs[type] = config;
        }

        _context = ActionContext.FromPlayer(this);
        _modifiers.Init(_playerData, _stats);
        LoadBasicStats();
    }

    public void UpdateStats()
    {
        _playerPRJCaster.TakeStats(_stats.Damage, _stats.ShootDelay, _currentWeaponConfig.GetPJCount(_stats.ProjectileCountStep));
        UpdateHP();
    }

    private void UpdateHP()
    {
        _maxHealth = Stats.MaxHP;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        StartDrawingBar();
    }

    public override void DealDamage(float damage, Vector3 closestPoint = default)
    {
        base.DealDamage(damage, closestPoint);
        Stats.CurrentHP = _currentHealth;
        _lastHitTime = Time.time;

        if (_currentHealth > 0)
        {
            FlashHitAnim(_noHitDuration);
            if (_cameraShake != null)
                _cameraShake.Shake();
        }

        TriggerEvent(PlayerEventType.OnDamageTake);
    }

    public void Heal()
    {
        _currentHealth += (Stats.MaxHP * 0.3f);
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        Stats.CurrentHP = _currentHealth;
        StartDrawingBar();

        TriggerEvent(PlayerEventType.OnHeal);
    }

    public override bool CanDamage()
    {
        return Time.time - _lastHitTime > _noHitDuration;
    }

    protected override void Death()
    {
        base.Death();
        OnPlayerDied?.Invoke();
    }

    public void SetPJSpawnPattern(Type pattern)
    {
        if (_weaponConfigs.TryGetValue(pattern, out var config))
        {
            _playerPRJCaster.SetShootPattern(config.SpawnPattern, config.DirGenerator, config.PJLifeTime);
            _currentWeaponConfig = config;
        }
        else
            Debug.Log($"Config of type {pattern} not found");
    }

    public void SetPJMovementType(Type moveType)
    {
        if (_PJMoveConfigs.TryGetValue(moveType, out var config))
        {
            _playerPRJCaster.SetPJMovementType(config.MovementType);
            _currentPJMoveConfig = config;
        }
        else
            Debug.Log($"Config of type {moveType} not found");
    }

    public void AddEvent(Player.EventSetup eventSetup)
    {
        _eventSetups.Add(eventSetup);
    }

    public void AddXP(int amount, float difficulty)
    {
        _stats.XP += math.abs(amount);

        int addScore = (int)(amount * 10 * difficulty);
        OnScoreChanged?.Invoke(_stats.Score, addScore);
        _stats.Score += addScore;

        if (_stats.XP >= _stats.LevelXPCost)
        {
            _stats.XP = 0;
            _stats.Level += 1;
            _stats.LevelXPCost = (int)(_stats.LevelXPCost * _playerData.levelMultiplier);

            OnLevelUp?.Invoke();
        }
        OnXPChanged?.Invoke(_stats.XP, _stats.LevelXPCost);
    }

    public void LoadBasicStats()
    {
        base.InitHP(_playerData.maxHealth);
        _stats.LoadFromData(_playerData);

        _modifiers.ClearAll();

        SetPJSpawnPattern(_playerData.weaponConfig.SpawnPattern.GetType());
        SetPJMovementType(_playerData.PJmoveConfig.MovementType.GetType());

        UpdateStats();
    }

    public void IsShooting(bool isShooting) { _playerPRJCaster.IsShooting(isShooting); }

    void IHitHandler.OnHit(Vector3 pos)
    {
        TriggerEvent(PlayerEventType.OnHit);
    }

    private void TriggerEvent(PlayerEventType eventType)
    {
        var setup = _eventSetups.Find(e => e.eventType == eventType);
        if (setup != null)
        {
            foreach (var action in setup.actions)
            {
                action.Execute(_context);
            }
        }
    }

    private void FlashHitAnim(float duration)
    {
        Color origColor = _sprite.color;
        _sprite.DOColor(_hitColor, 0.2f).SetLoops((int)math.ceil(duration / 0.2f), LoopType.Yoyo).OnComplete(() => _sprite.color = origColor);
    }

    public Tween RestartAnimScaleFade()
    {
        _playerPRJCaster.IsShooting(false);
        Vector2 pos = new Vector2(_startPosition.x, _startPosition.y - 20);
        return transform.DOScale(new Vector3(0, 0, _originalScale.z), _animDuration).SetUpdate(true).SetEase(_fadeCurve).OnComplete(() => SetScaleAndPos(pos));
    }

    public Tween RestartAnimStartPos()
    {
        return transform.DOMove(_startPosition, _animDuration).SetUpdate(true).SetEase(_appearCurve).OnComplete(() => _playerPRJCaster.IsShooting(true));
    }

    private void SetScaleAndPos(Vector2 pos)
    {
        transform.position = pos;
        transform.localScale = _originalScale;
    }

    protected override void Start()
    {
        base.Start();
        _playerPRJCaster.IsShooting(true);
        _startPosition = transform.position;
        _originalScale = transform.localScale;
    }
    private void Update()
    {
        _playerCanvas.transform.position = Vector3.SmoothDamp(_playerCanvas.transform.position, transform.position, ref v, 1 / _canvasFollowSpeed);

        _currentSpeed = Vector3.Distance(transform.position, _previousPositon) / Time.deltaTime;
        _previousPositon = transform.position;

        if (_currentSpeed > 2 && Time.time - _onMoveLastTrigger >= _onMoveTriggerDelay)
        {
            TriggerEvent(PlayerEventType.OnMove);
            _onMoveLastTrigger = Time.time;
        }
    }
}