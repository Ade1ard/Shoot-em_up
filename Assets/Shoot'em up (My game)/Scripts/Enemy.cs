using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Enemy : Health
{
    [SerializeField] private int _xpReward = 10;

    public event Action<Enemy, int> OnDeath;

    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<Animator>().SetFloat("StartOffset", UnityEngine.Random.Range(0f, 1f));
    }

    protected override void Death()
    {
        base.Death();
        OnDeath?.Invoke(this, _xpReward);
        DOTween.Kill(transform);
        Destroy(gameObject);
    }
}