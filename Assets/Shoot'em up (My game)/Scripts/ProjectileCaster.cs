using UnityEngine;

public class ProjectileCaster : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] ProjectileCont _projectilePrefab;

    [Header("ProjectileSettings")]
    [SerializeField] private float _PRJspeed;
    [SerializeField] private float _PRJdamage;

    [Header("OwnSettings")]
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _shootDelay;
    [SerializeField] private ParticleSystem _shootVFXPrefab;
    [SerializeField] Animator _shootFlashAnimator;

    private float _lastShootTime;

    private Rigidbody2D _playerRigidBoby;

    private void Start()
    {
        _playerRigidBoby = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Time.time - _lastShootTime > _shootDelay)
        {
            _lastShootTime = Time.time;

            Vector3 InitPos = new Vector3(_shootPoint.position.x, _shootPoint.position.y, 0);
            Instantiate(_projectilePrefab, InitPos, Quaternion.identity).Initialize(_PRJspeed, _PRJdamage, _playerRigidBoby.linearVelocity);

            if (_shootVFXPrefab != null)
                Instantiate(_shootVFXPrefab, _shootPoint.position, Quaternion.identity, transform);

            if (_shootFlashAnimator != null)
                _shootFlashAnimator.SetTrigger("Shoot");
        }
    }
}
