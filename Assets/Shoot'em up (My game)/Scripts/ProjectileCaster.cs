using UnityEngine;

public class ProjectileCaster : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] ProjectileCont _projectilePrefab;

    [Header("ProjectileSettings")]
    [SerializeField] private float _PRJspeed;

    [Header("OwnSettings")]
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _shootDelay;
    [SerializeField] private ParticleSystem _shootVFXPrefab;
    [SerializeField] Animator _shootFlashAnimator;

    private float _lastShootTime;

    void Update()
    {
        if (Time.time - _lastShootTime > _shootDelay)
        {
            _lastShootTime = Time.time;

            Instantiate(_projectilePrefab, _shootPoint.position, Quaternion.identity).Initialize(_PRJspeed);

            if (_shootVFXPrefab != null)
                Instantiate(_shootVFXPrefab, _shootPoint.position, Quaternion.identity, transform);

            if (_shootFlashAnimator != null)
                _shootFlashAnimator.SetTrigger("Shoot");
        }
    }
}
