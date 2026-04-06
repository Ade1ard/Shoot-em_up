using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ProjectileCaster))]
public class PlayerStats : Health
{
    public int damage = 10;
    public float shootDelay = 1f;
    public int projectileCount = 1;
    public int levelXPCost = 100;
    public float levelMultiplier = 1.2f;

    [Header("Canvas")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private float _canvasFollowSpeed = 35;

    [NonSerialized] public UnityEvent onPlayerDied;
    [NonSerialized] public int level = 0;
    [NonSerialized] public int score = 0;

    private CardSelectionManager _cardManager;
    private ProjectileCaster _playerPRJCaster;
    private UIView _UIView;
    private ScoreUI _scoreUI;

    private Vector3 v = Vector3.zero;
    private int XP;

    protected override void Start()
    {
        base.Start();

        _cardManager = FindAnyObjectByType<CardSelectionManager>();
        _UIView = FindAnyObjectByType<UIView>();
        _playerPRJCaster = GetComponent<ProjectileCaster>();
        _scoreUI = FindAnyObjectByType<ScoreUI>();
        UpdateStats();
        UpdateProjectileCount();
    }

    public void UpdateStats() { _playerPRJCaster.GetStats(damage, shootDelay, projectileCount); }
    public void UpdateProjectileCount() { _playerPRJCaster.GetShootPoints(projectileCount); }

    protected override void Death()
    {
        base.Death();
        onPlayerDied?.Invoke();
    }

    private void Update()
    {
        _playerCanvas.transform.position = Vector3.SmoothDamp(_playerCanvas.transform.position, transform.position, ref v, 1 / _canvasFollowSpeed);
    }

    public void AddXP(int amount, float difficulty)
    {
        XP += math.abs(amount);
        _UIView.StartDrawingBar(XP, levelXPCost);

        score += (int)(amount * 10 * difficulty);
        _scoreUI.UpdateScoreAmount(score);

        if (XP >= levelXPCost)
        {
            XP = 0;
            level += 1;
            levelXPCost = (int)(levelXPCost * levelMultiplier);

            _cardManager.ShowCardSelection();
            _UIView.StartDrawingBar(XP, levelXPCost);
        }
    }
}