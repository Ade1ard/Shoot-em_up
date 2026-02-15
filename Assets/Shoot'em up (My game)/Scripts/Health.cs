using UnityEngine;

public abstract class Health : MonoBehaviour, IDamageable
{
    [Header("Parameters")]
    [SerializeField] private float _maxHealth;

    private float _currentHealth;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void Initialize(float multiplier = default)
    {
        if (multiplier != 0)
            _maxHealth *= multiplier;

        _currentHealth = _maxHealth;
    }

    public virtual void DealDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        if (_currentHealth <= 0f)
            Death();
    }

    protected virtual void Death()
    {

    }
}

public interface IDamageable
{
    void DealDamage(float damage);
}