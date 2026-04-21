using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ProjectileCaster))]
public class Player : Health, IInitializable
{
    [Header("Canvas")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private float _canvasFollowSpeed = 35;

    [NonSerialized] public int level = 0;
    [NonSerialized] public int score = 0;
    private int XP;

    private PlayerData _playerData;

    private int damageMod = 10;
    private float shootDelayMod = 1f;
    private int projectileCountMod = 1;
    private int levelXPCostMod = 100;
    private float levelMultiplierMod = 1.2f;

    private ProjectileCaster _playerPRJCaster;
    private UIView _UIView;
    private ScoreUI _scoreUI;

    public Action OnLevelUp;
    public Action OnPlayerDied;

    private Vector3 v = Vector3.zero;

    public void Init()
    {
        _UIView = G.Get<UIView>();
        _scoreUI = G.Get<ScoreUI>();

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
        _UIView.StartDrawingBar(XP, levelXPCostMod);

        int addScore = (int)(amount * 10 * difficulty);
        _scoreUI.UpdateScoreAmount(score, addScore);
        score += addScore;

        if (XP >= levelXPCostMod)
        {
            XP = 0;
            level += 1;
            levelXPCostMod = (int)(levelXPCostMod * levelMultiplierMod);

            OnLevelUp?.Invoke();
            _UIView.StartDrawingBar(XP, levelXPCostMod);
        }
    }

    public void LoadBasicStats()
    {
        base.InitHP(_playerData.maxHealth);
        shootDelayMod = _playerData.shootDelay;
        projectileCountMod = _playerData.projectileCount;
        levelXPCostMod = _playerData.levelXPCost;
        levelMultiplierMod = _playerData.levelMultiplier;

        UpdateStats();
        UpdateProjectileCount();
    }

    public void UIVIsible(bool visible)
    {
        _healthBarCanvasGroup.gameObject.SetActive(visible);
    }
}