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

    public void Initialize(float multiplier = default)
    {
        if (multiplier != 0)
            _maxHealth *= multiplier;

        _currentHealth = _maxHealth;
    }

    protected override void Death()
    {
        DOTween.Kill(transform);
        base.Death();
        OnDeath?.Invoke(this, _xpReward);
        Destroy(gameObject, 0.1f);
    }

    public void UIVisible(bool visible)
    {
        _healthBarCanvasGroup.gameObject.SetActive(visible);
    }
}