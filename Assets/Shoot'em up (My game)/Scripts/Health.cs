using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Health : MonoBehaviour, IDamageable
{
    [Header("UI")]
    [SerializeField] private Image _healthBarAmount;
    [SerializeField] private float _healthBarDrawingSpeed = 1;

    [Header("Parameters")]
    [SerializeField] private float _maxHealth;

    private float _currentHealth;
    private Coroutine _healthBarCoroutine;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        StartDrawingBar();
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

        StartDrawingBar();

        if (_currentHealth <= 0f)
            Death();
    }

    protected virtual void Death()
    {

    }

    private void StartDrawingBar()
    {
        if (_healthBarCoroutine == null)
            _healthBarCoroutine = StartCoroutine(DrawHealthBar());
    }

    private IEnumerator DrawHealthBar()
    {
        while (_healthBarAmount.fillAmount != (_currentHealth / _maxHealth))
        {
            _healthBarAmount.fillAmount = Mathf.MoveTowards(_healthBarAmount.fillAmount, _currentHealth / _maxHealth, _healthBarDrawingSpeed / 100);
            yield return null;
        }
        _healthBarCoroutine = null;
    }
}

public interface IDamageable
{
    void DealDamage(float damage);
}