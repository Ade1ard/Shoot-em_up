using DG.Tweening;
using System;
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

    [NonSerialized] public int level = 1;
    [NonSerialized] public int score = 0;
    private int XP;

    private PlayerData _playerData;

    private int damageMod = 10;
    private float shootDelayMod = 1f;
    private int projectileCountMod = 1;
    private int levelXPCostMod = 100;
    private float levelMultiplierMod = 1.2f;

    private ProjectileCaster _playerPRJCaster;

    public Action OnLevelUp;
    public Action OnPlayerDied;
    public Action<int, int> OnXPChanged;
    public Action<int, int> OnScoreChanged;

    private Vector3 v = Vector3.zero;
    private Vector2 _startPosition;
    private Vector3 _originalScale;

    public void Init()
    {
        _playerPRJCaster = GetComponent<ProjectileCaster>();
        _playerData = Resources.Load<PlayerData>("Player/PlayerData");

        LoadBasicStats();
        UpdateStats();
        UpdateProjectileCount();
    }

    public void UpdateStats() { _playerPRJCaster.TakeStats(damageMod, shootDelayMod, projectileCountMod); }
    public void UpdateProjectileCount() { _playerPRJCaster.ChangeShootPoints(projectileCountMod); }

    protected override void Death()
    {
        base.Death();
        OnPlayerDied?.Invoke();
    }

    private void Update()
    {
        _playerCanvas.transform.position = Vector3.SmoothDamp(_playerCanvas.transform.position, transform.position, ref v, 1 / _canvasFollowSpeed);
    }

    public void AddMaxHP(int amount)
    {
        _maxHealth += amount;
        _currentHealth += amount;
    }

    public void AddDamage(int amount)
    {
        damageMod += amount;
    }

    public void AddAttackSpeed(float amount)
    {
        shootDelayMod -= amount;
        shootDelayMod = Mathf.Clamp(shootDelayMod, 0.17f, 1);
    }

    public void AddPrjcCount()
    {
        projectileCountMod += 1;
        projectileCountMod = Mathf.Clamp(projectileCountMod, 1, 3);
        UpdateProjectileCount();
    }

    public void AddXP(int amount, float difficulty)
    {
        XP += math.abs(amount);

        int addScore = (int)(amount * 10 * difficulty);
        OnScoreChanged?.Invoke(score, addScore);
        score += addScore;

        if (XP >= levelXPCostMod)
        {
            XP = 0;
            level += 1;
            levelXPCostMod = (int)(levelXPCostMod * levelMultiplierMod);

            OnLevelUp?.Invoke();
        }
        OnXPChanged?.Invoke(XP, levelXPCostMod);
    }

    public void LoadBasicStats()
    {
        base.InitHP(_playerData.maxHealth);
        shootDelayMod = _playerData.shootDelay;
        projectileCountMod = _playerData.projectileCount;
        levelXPCostMod = _playerData.levelXPCost;
        levelMultiplierMod = _playerData.levelMultiplier;

        level = 1;
        score = 0;
        XP = 0;

        UpdateStats();
        UpdateProjectileCount();
    }

    public Tween RestartAnimScaleFade()
    {
        Vector2 pos = new Vector2(_startPosition.x, _startPosition.y - 20);
        return transform.DOScale(new Vector3(0, 0, _originalScale.z), _animDuration).SetUpdate(true).SetEase(_fadeCurve).OnComplete(() => SetScaleAndPos(pos));
    }

    public Tween RestartAnimStartPos()
    {
        return transform.DOMove(_startPosition, _animDuration).SetUpdate(true).SetEase(_appearCurve);
    }

    private void SetScaleAndPos(Vector2 pos)
    {
        transform.position = pos;
        transform.localScale = _originalScale;
    }

    protected override void Start()
    {
        base.Start();
        _startPosition = transform.position;
        _originalScale = transform.localScale;
    }
}