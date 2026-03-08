using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ProjectileCaster))]
public class PlayerStats : Health
{
    public int damage = 10;
    public float attackSpeed = 1f;
    public int projectileCount = 1;
    private int level = 0;
    public int levelXPCost = 100;

    [Header("Canvas")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private float _canvasFollowSpeed = 35;

    [NonSerialized] public UnityEvent onPlayerDied;

    private CardSelectionManager _cardManager;

    private Vector3 v = Vector3.zero;
    private int XP;

    protected override void Start()
    {
        base.Start();

        _cardManager = FindAnyObjectByType<CardSelectionManager>();
    }

    protected override void Death()
    {
        base.Death();
        onPlayerDied?.Invoke();
    }

    private void Update()
    {
        _playerCanvas.transform.position = Vector3.SmoothDamp(_playerCanvas.transform.position, transform.position, ref v, 1 / _canvasFollowSpeed);
    }

    public void AddXP(int amount)
    {
        XP += math.abs(amount);

        if (XP >= levelXPCost)
        {
            XP = 0;
            level += 1;

            _cardManager.ShowCardSelection();
        }
    }
}