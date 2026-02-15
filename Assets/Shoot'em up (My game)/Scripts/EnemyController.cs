using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class EnemyController : Health
{
    [SerializeField] private int _xpReward = 10;

    public event Action<GameObject, int> OnDeath;

    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<Animator>().SetFloat("StartOffset", UnityEngine.Random.Range(0f, 1f));
    }

    protected override void Death()
    {
        OnDeath?.Invoke(gameObject, _xpReward);
        DOTween.Kill(transform);
        Destroy(gameObject);
    }
}