using DG.Tweening;
using System;
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
        _projectileCaster.TakeStats(PRJDamage, shootDelay, projectileCount);
        _projectileCaster.IsShooting(true);
    }

    protected override void Death()
    {
        base.UIVisible(false);
        DOTween.Kill(transform);
        base.Death();
        OnDeath?.Invoke(this, xpReward);
        Destroy(gameObject, 0.1f);
    }
}