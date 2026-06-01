using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ProjectileCaster))]
public class Player : Health, IInitializable
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

    private PlayerData _playerData;
    private PlayerRuntimeStats _stats;
    public PlayerRuntimeStats Stats => _stats;
    private PlayerModifiers _modifiers;
    public PlayerModifiers Modifiers => _modifiers;
    private Dictionary<Type, PlayerWeaponConfig> _weaponConfigs = new Dictionary<Type, PlayerWeaponConfig>();

    private PlayerWeaponConfig _currentWeaponConfig;
    public PlayerWeaponConfig CurrentWeaponConfig => _currentWeaponConfig;

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

    public void Init()
    {
        _modifiers = new PlayerModifiers();
        _stats = new PlayerRuntimeStats();
        _playerPRJCaster = GetComponent<ProjectileCaster>();
        _sprite = GetComponent<SpriteRenderer>();
        _cameraShake = G.Get<CameraShake>();

        _playerData = Resources.Load<PlayerData>("Player/PlayerData");
        var configs = Resources.LoadAll<PlayerWeaponConfig>("Player").ToList();
        foreach (var config in configs)
        {
            var type = config.SpawnPattern.GetType();
            _weaponConfigs[type] = config;
        }

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
    }

    public void Heal()
    {
        _currentHealth += (Stats.MaxHP * 0.3f);
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        Stats.CurrentHP = _currentHealth;
        StartDrawingBar();
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
            _playerPRJCaster.SetShootPattern(config.SpawnPattern, config.DirGenerator);
            _currentWeaponConfig = config;
        }
        else
            Debug.Log($"Config of type {pattern} not found");
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

        var config = _playerData.weaponConfig;
        _playerPRJCaster.SetShootPattern(config.SpawnPattern, config.DirGenerator);
        _currentWeaponConfig = config;
        UpdateStats();
    }

    public void IsShooting(bool isShooting) { _playerPRJCaster.IsShooting(isShooting); }

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
    }
}