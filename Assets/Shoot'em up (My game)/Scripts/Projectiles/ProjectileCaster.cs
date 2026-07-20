using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProjectileCaster : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] ProjectileCont _projectilePrefab;

    [Header("Settings")]
    [SerializeField] Transform _shootPoint;
    [SerializeField] ParticleSystem _shootVFXPrefab;
    [SerializeField] Animator _shootFlashAnimator;

    [Header("SpawnPattern")]
    [SerializeReference, SubclassSelector]
    private ISpawnFormation _spawnPattern;

    [Header("Offsets")]
    [SerializeField] private float _spawnPositonOffset_X = 0.1f;
    [SerializeField] private float _spawnPositonOffset_Y = 0.1f;
    [SerializeField] private float _shootDelayOffset = 0.1f;

    [Header("Sound")]
    [SerializeField] private List<AudioClip> _shootSounds = new List<AudioClip>();
    private float lastSoundTime;

    private float _PRJDamage;
    private float? _PRJLifeTime;
    private float _shootDelay;
    private int _projectileCount;
    private float _lastShootTime;
    private bool _isShooting;
    private List<Transform> _shootP = new List<Transform>();
    private AudioSource _audioSource;
    private IHitHandler _hitHandler;
    private ProjectilePool _projectilePool;
    private ProjectileOwner _projectileOwner = ProjectileOwner.Other;

    private void Start()
    {
        if (_shootPoint == null)
            _shootPoint = transform;

        _lastShootTime = Time.time;
        UpdateShootPoints(_projectileCount);

        _audioSource = GetComponent<AudioSource>();
        _hitHandler = GetComponent<IHitHandler>();
        _projectilePool = G.Get<ProjectilePool>();
        _projectileOwner = GetComponent<Player>() != null ? ProjectileOwner.Player :
            GetComponent<Enemy>() != null ? ProjectileOwner.Enemy : ProjectileOwner.Other;
    }

    public void IsShooting(bool isShooting) { _isShooting  = isShooting; }

    public void SetShootPattern(ISpawnFormation spawnFormation, IDirectionGenerator dirGenerator = null, float? PJlifeTime = null)
    {
        _spawnPattern = spawnFormation;

        if (dirGenerator != null)
            _projectilePrefab.GetComponent<ObjectMovement>().Init(dirGenerator);
        if (PJlifeTime.HasValue)
            _PRJLifeTime = PJlifeTime.Value;
        
        if (_projectileOwner == ProjectileOwner.Player && _projectilePool != null)
            _projectilePool.ClearActive(ProjectileOwner.Player);

        _projectilePool?.ClearInactive(_projectilePrefab);
    }

    public void SetPJMovementType(IMovementType movementType)
    {
        _projectilePrefab.GetComponent<ObjectMovement>().Init(null, movementType);

        if (_projectileOwner == ProjectileOwner.Player && _projectilePool != null)
            _projectilePool.ClearActive(ProjectileOwner.Player);

        _projectilePool?.ClearInactive(_projectilePrefab);
    }

    public void TakeStats(float damage, float shootDelay, int projectileCount)
    {
        _PRJDamage = damage;
        _shootDelay = shootDelay;
        _projectileCount = projectileCount;
       UpdateShootPoints(_projectileCount);
    }

    void Update()
    {
        if (!_isShooting) return;

        if (Time.time - _lastShootTime > _shootDelay && OnScreen())
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        foreach (var p in _shootP)
        {
            _lastShootTime = Time.time;
            _lastShootTime += Random.Range(-_shootDelayOffset, _shootDelayOffset);

            Vector3 InitPos = new Vector3(
                p.position.x + Random.Range(-_spawnPositonOffset_X, _spawnPositonOffset_X),
                p.position.y + Random.Range(-_spawnPositonOffset_Y, _spawnPositonOffset_Y),
                0);

            var projectile = GetProjectile(_projectilePrefab, InitPos, _projectileOwner);
            projectile.Initialize(_PRJDamage, _PRJLifeTime, _hitHandler);
            
            MovementContext context = new MovementContext(InitPos, _shootPoint.position, transform);
            projectile.GetComponent<ObjectMovement>().StartMove(context);
        }

        if (_shootSounds.Count != 0 && Time.time - lastSoundTime >= 0.1f)
        {
            _audioSource.PlayOneShot(_shootSounds[Random.Range(0, _shootSounds.Count)]);
            lastSoundTime = Time.time;
        }

        if (_shootVFXPrefab != null)
        {
            var vfx = G.VFXPool.Get(_shootVFXPrefab, _shootPoint.position, Quaternion.identity, transform);
            vfx.Play();
        }

        if (_shootFlashAnimator != null)
            _shootFlashAnimator.SetTrigger("Shoot");
    }

    public void CustomShoot(CustomShootContext context, Vector3? spawnPos = null)
    {
        GameObject ShootPoint = new GameObject($"{gameObject.name}ShootPoint");
        ShootPoint.transform.position = spawnPos.HasValue ? spawnPos.Value : transform.position;

        for (int i = 0; i < context._PJCount; i++)
        {
            GameObject bullet = new GameObject($"{gameObject.name}ShootPoint{i}");
            bullet.transform.SetParent(ShootPoint.transform);
            bullet.transform.localPosition = context._spawnPattern.CalculateSpawnPosition(Vector3.zero, i, context._PJCount);

            var PJ = GetProjectile(context._projectilePrefab, bullet.transform.position, _projectileOwner);
            PJ.Initialize(context._PJDamage, context._PJLifeTime);
            
            MovementContext moveContext = new MovementContext(bullet.transform.position, ShootPoint.transform.position, transform);
            ObjectMovement ob = PJ.GetComponent<ObjectMovement>();
                if (ob != null)
                    ob.StartMove(moveContext);

            if (context._vFX != null)
            {
                var vfx = G.VFXPool.Get(context._vFX, PJ.transform.position, Quaternion.identity);
                vfx.Play();
            }
                
            if (context._sound != null)
                _audioSource.PlayOneShot(context._sound);
        }

        Destroy(ShootPoint.gameObject);
    }

    private ProjectileCont GetProjectile(ProjectileCont prefab, Vector3 position, ProjectileOwner owner)
    {
        if (_projectilePool == null)
            _projectilePool = G.Get<ProjectilePool>();

        ProjectileCont projectile = _projectilePool.Get(prefab, position, Quaternion.Euler(0,0,0), owner);
        return projectile;
    }

    private void UpdateShootPoints(int count)
    {
        foreach (Transform p in _shootP)
            Destroy(p.gameObject);
        _shootP.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject bullet = new GameObject($"{gameObject.name}ShootPoint{i}");
            bullet.transform.SetParent(_shootPoint);
            bullet.transform.localPosition = _spawnPattern.CalculateSpawnPosition(Vector3.zero, i, _projectileCount);
            _shootP.Add(bullet.transform);
        }
    }

    bool OnScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
            viewportPos.y >= 0 && viewportPos.y <= 1;
    }
}
