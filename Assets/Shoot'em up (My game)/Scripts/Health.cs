using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public abstract class Health : MonoBehaviour, IDamageable
{
    [Header("UI")]
    [SerializeField] private Image _healthBarAmount;
    public CanvasGroup _healthBarCanvasGroup;
    [SerializeField] private float _healthBarDrawingSpeed = 1;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _deathVFX;
    [SerializeField] private ParticleSystem _takeDamageVFX;

    [Header("Sounds")]
    [SerializeField] private List<AudioClip> _hitSounds = new List<AudioClip>();
    [SerializeField] private AudioClip _deadSound;

    [NonSerialized] public float _maxHealth;
    [NonSerialized] public float _currentHealth;
    private Coroutine _healthBarCoroutine;
    private AudioSource _audioSource;

    protected virtual void InitHP(float maxhealth)
    {
        _maxHealth = maxhealth;
        _currentHealth = maxhealth;
        StartDrawingBar();
    }

    protected virtual void Start()
    {
        _healthBarCanvasGroup.gameObject.SetActive(false);

        _audioSource = GetComponent<AudioSource>();
    }

    public virtual void DealDamage(float damage, Vector3 closestPoint = default)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

        _healthBarCanvasGroup.gameObject.SetActive(true);
        StartDrawingBar();

        if (_hitSounds.Count != 0)
            _audioSource.PlayOneShot(_hitSounds[UnityEngine.Random.Range(0, _hitSounds.Count)]);

        if (_currentHealth <= 0f)
            Death();
        else if (closestPoint != default)
            Instantiate(_takeDamageVFX, closestPoint, Quaternion.identity);
    }

    protected virtual void Death()
    {
        if (_deathVFX != null)
            Instantiate(_deathVFX, transform.position, Quaternion.identity);
        if (_deadSound != null)
            AudioSource.PlayClipAtPoint(_deadSound, transform.position, _audioSource.volume);
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
    void DealDamage(float damage, Vector3 closestPoint = default);
}