using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class EnemyController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private int _xpReward;

    private float _currentHealth;

    public event Action<GameObject, int> OnDeath;

    void Start()
    {
        GetComponentInChildren<Animator>().SetFloat("StartOffset", UnityEngine.Random.Range(0f, 1f));
        _currentHealth = _maxHealth;
    }

    public void Initialize(float difficulty)
    {
        _maxHealth *= difficulty;
        _currentHealth = _maxHealth;
    }

    public void DealDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        if (_currentHealth <= 0f)
        {
            OnDeath?.Invoke(gameObject, _xpReward);
            DOTween.Kill(transform);
            Destroy(gameObject);
        }
    }
}