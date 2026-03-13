using UnityEngine;

public class ProjectileCaster : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] ProjectileCont _projectilePrefab;

    [Header("Stats")]
    [SerializeField] private float _PRJDamage;
    [SerializeField] private float _shootDelay;
    [SerializeField] private int _projectileCount = 1;

    [Header("Settings")]
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private ParticleSystem _shootVFXPrefab;
    [SerializeField] Animator _shootFlashAnimator;

    [Header("Offsets")]
    [SerializeField] private float _spawnPositonOffset_X = 0.1f;
    [SerializeField] private float _spawnPositonOffset_Y = 0.1f;
    [SerializeField] private float _shootDelayOffset = 0.1f;

    private float _lastShootTime;

    private void Start()
    {
        if (_shootPoint == null)
            _shootPoint = transform;

        _lastShootTime = Time.time;
    }

    public void GetStats(float damage, float shootDelay, int projectileCount) //for Player
    {
        _PRJDamage = damage;
        _shootDelay = shootDelay;
        _projectileCount = projectileCount;
    }

    void Update()
    {
        if (Time.time - _lastShootTime > _shootDelay && OnScreen())
        {
            _lastShootTime = Time.time;
            _lastShootTime += Random.Range(-_shootDelayOffset, _shootDelayOffset);

            Vector3 InitPos = new Vector3(_shootPoint.position.x + Random.Range(-_spawnPositonOffset_X, _spawnPositonOffset_X),
                _shootPoint.position.y + Random.Range(-_spawnPositonOffset_Y, _spawnPositonOffset_Y),
                0);

            var projectile = Instantiate(_projectilePrefab, InitPos, Quaternion.identity);
            projectile.Initialize(_PRJDamage);
            projectile.GetComponent<ObjectMovement>().StartMove(InitPos);

            if (_shootVFXPrefab != null)
                Instantiate(_shootVFXPrefab, _shootPoint.position, Quaternion.identity, transform);

            if (_shootFlashAnimator != null)
                _shootFlashAnimator.SetTrigger("Shoot");
        }
    }

    bool OnScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
            viewportPos.y >= 0 && viewportPos.y <= 1;
    }
}
