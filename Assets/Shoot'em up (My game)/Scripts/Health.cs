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
    private float lastSoundTime;

    [NonSerialized] public float _maxHealth;

    private float _hp;
    public event Action OnHpChanged;

    public float currentHealth
    {
        get => _hp;
        set
        {
            _hp = value;
            OnHpChanged?.Invoke();
        }
    }

    private Coroutine _healthBarCoroutine;
    private AudioSource _audioSource;

    protected virtual void InitHP(float maxhealth)
    {
        _maxHealth = maxhealth;
        currentHealth = maxhealth;
    }

    protected virtual void Start()
    {
        _healthBarCanvasGroup.gameObject.SetActive(false);

        _audioSource = GetComponent<AudioSource>();
    }

    public virtual void DealDamage(float damage, Vector3 closestPoint = default)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, _maxHealth);

        _healthBarCanvasGroup.gameObject.SetActive(true);

        if (_hitSounds.Count != 0 && Time.time - lastSoundTime >= 0.1f)
        {
            _audioSource.PlayOneShot(_hitSounds[UnityEngine.Random.Range(0, _hitSounds.Count)]);
            lastSoundTime = Time.time;
        }

        if (currentHealth <= 0f)
            Death();
        else if (closestPoint != default)
            Instantiate(_takeDamageVFX, closestPoint, Quaternion.identity);
    }

    public virtual bool CanDamage() { return true; }

    public virtual void ChangeHP(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, _maxHealth);

        if (currentHealth <= 0f)
            Death();
    }

    protected virtual void Death()
    {
        if (_deathVFX != null)
            Instantiate(_deathVFX, transform.position, Quaternion.identity);
        if (_deadSound != null)
            AudioSource.PlayClipAtPoint(_deadSound, transform.position, _audioSource.volume);
    }

    public void StartDrawingBar()
    {
        if (_healthBarCoroutine == null)
            _healthBarCoroutine = StartCoroutine(DrawHealthBar());
    }

    private IEnumerator DrawHealthBar()
    {
        while (_healthBarAmount.fillAmount != (currentHealth / _maxHealth))
        {
            _healthBarAmount.fillAmount = Mathf.MoveTowards(_healthBarAmount.fillAmount, currentHealth / _maxHealth, _healthBarDrawingSpeed / 100);
            yield return null;
        }
        _healthBarCoroutine = null;
    }

    public void UIVisible(bool visible)
    {
        if (visible && currentHealth == _maxHealth) return;

        _healthBarCanvasGroup.gameObject.SetActive(visible);
    }

    private void UIVis() { UIVisible(true); }

    protected virtual void OnEnable()
    {
        OnHpChanged += StartDrawingBar;
        OnHpChanged += UIVis;
    }

    protected virtual void OnDisable()
    {
        OnHpChanged -= StartDrawingBar;
        OnHpChanged -= UIVis;
    }
}

public interface IDamageable
{
    void DealDamage(float damage, Vector3 closestPoint = default);
    bool CanDamage();
}