using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ProjectileCaster))]
public class Player : Health, IInitializable
{
    public int damage = 10;
    public float shootDelay = 1f;
    public int projectileCount = 1;
    public int levelXPCost = 100;
    public float levelMultiplier = 1.2f;

    [Header("Canvas")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private float _canvasFollowSpeed = 35;

    [NonSerialized] public int level = 0;
    [NonSerialized] public int score = 0;

    private ProjectileCaster _playerPRJCaster;
    private UIView _UIView;
    private ScoreUI _scoreUI;

    public Action OnLevelUp;
    public Action OnPlayerDied;

    private Vector3 v = Vector3.zero;
    private int XP;

    public void Init()
    {
        _UIView = G.Get<UIView>();
        _scoreUI = G.Get<ScoreUI>();

        _playerPRJCaster = GetComponent<ProjectileCaster>();

        UpdateStats();
        UpdateProjectileCount();
    }

    public void UpdateStats() { _playerPRJCaster.TakeStats(damage, shootDelay, projectileCount); }
    public void UpdateProjectileCount() { _playerPRJCaster.ChangeShootPoints(projectileCount); }

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
        damage += amount;
    }

    public void AddAttackSpeed(float amount)
    {
        shootDelay -= amount;
        shootDelay = Mathf.Clamp(shootDelay, 0.17f, 1);
    }

    public void AddPrjcCount()
    {
        projectileCount += 1;
        projectileCount = Mathf.Clamp(projectileCount, 1, 3);
        UpdateProjectileCount();
    }

    public void AddXP(int amount, float difficulty)
    {
        XP += math.abs(amount);
        _UIView.StartDrawingBar(XP, levelXPCost);

        int addScore = (int)(amount * 10 * difficulty);
        _scoreUI.UpdateScoreAmount(score, addScore);
        score += addScore;

        if (XP >= levelXPCost)
        {
            XP = 0;
            level += 1;
            levelXPCost = (int)(levelXPCost * levelMultiplier);

            OnLevelUp?.Invoke();
            _UIView.StartDrawingBar(XP, levelXPCost);
        }
    }

    public void ReloadStats()
    {

    }

    public void UIVIsible(bool visible)
    {
        _healthBarCanvasGroup.gameObject.SetActive(visible);
    }
}