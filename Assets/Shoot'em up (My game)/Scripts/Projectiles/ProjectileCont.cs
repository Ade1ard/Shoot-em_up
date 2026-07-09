using System;
using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class ProjectileCont : MonoBehaviour
{
    [SerializeField] private ParticleSystem _clearVFX;
    [SerializeField] private ParticleSystem _spawnVFX;
    [SerializeField] private float _lifeTime; // if == 0 endless lifeTime
    [Header("Flags")]
    [SerializeField] private bool _isItEnemy = false;
    [SerializeField] private bool _disposable = true;
    [SerializeField] private bool _clearable = true;

    private IHitHandler _hitHandler;

    private float _damage;
    private ProjectilePool _pool;
    private ProjectileCont _prefabKey;
    private Rigidbody2D _rb;
    private ObjectMovement _movement;
    private float _defaultLifeTime;
    private Coroutine _lifeTimeCoroutine;
    [NonSerialized] public Quaternion StartSpriteRotation;

    public ProjectileOwner Owner { get; private set; } = ProjectileOwner.Other;
    public ProjectileCont PrefabKey => _prefabKey;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<ObjectMovement>();
        _defaultLifeTime = _lifeTime;

        var sprite = GetComponentInChildren<SpriteRenderer>();
        StartSpriteRotation = sprite != null ? sprite.transform.localRotation : Quaternion.Euler(0, 0, 0);
        
        if (_spawnVFX != null)
            Instantiate(_spawnVFX, transform.position, Quaternion.identity);
    }

    public void SetPool(ProjectilePool pool, ProjectileCont prefabKey)
    {
        _pool = pool;
        _prefabKey = prefabKey;
    }

    public void PrepareFromPool(ProjectileOwner owner, ProjectileCont prefab)
    {
        StopLifeTimeCoroutine();
        Owner = owner;
        _lifeTime = prefab != null ? prefab._lifeTime : _defaultLifeTime;
        
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }

    public void Initialize(float? damage = null, float? lifeTime = null, IHitHandler hitHandler = null)
    {
        if (damage.HasValue)
            _damage = damage.Value;
        else
            _damage = 10;

        _hitHandler = hitHandler;

        if (lifeTime.HasValue)
            _lifeTime = lifeTime.Value;
        if (_lifeTime > 0)
            _lifeTimeCoroutine = StartCoroutine(LifeTimeRoutine(_lifeTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable ID) && OnScreen())
        {
            if (!ID.CanDamage()) return;

            ID.DealDamage(_damage, collision.ClosestPoint(transform.position));
            _hitHandler?.OnHit(transform.position);

            if (_isItEnemy || !_disposable) return;

            ReturnToPool(false);
        }
    }

    public void Clear()
    {
        if (_isItEnemy || !_disposable || !_clearable) return;
        ReturnToPool(true);
    }

    public void ReturnToPool(bool playClearVFX)
    {
        if (playClearVFX && _clearVFX != null)
            Instantiate(_clearVFX, transform.position, Quaternion.identity);
        
        if (_pool != null)
            _pool.Release(this);
        else
        {
            StopBeforePool();
            Destroy(gameObject);
        }
    }

    public void StopBeforePool()
    {
        StopLifeTimeCoroutine();
        _hitHandler = null;
        DOTween.Kill(transform);

        if (_movement == null)
            _movement = GetComponent<ObjectMovement>();

        if (_movement != null)
            _movement.StopMove();

        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }

    private IEnumerator LifeTimeRoutine(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        _lifeTimeCoroutine = null;
        ReturnToPool(true);
    }

    private void StopLifeTimeCoroutine()
    {
        if (_lifeTimeCoroutine == null) return;
        
        StopCoroutine(_lifeTimeCoroutine);
        _lifeTimeCoroutine = null;
    }

    bool OnScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
            viewportPos.y >= 0 && viewportPos.y <= 1;
    }
}