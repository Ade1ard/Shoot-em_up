using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProjectileCaster : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] ProjectileCont _projectilePrefab;

    [Header("Settings")]
    [SerializeField] Transform _shootPoints;
    [SerializeField] private ParticleSystem _shootVFXPrefab;
    [SerializeField] Animator _shootFlashAnimator;

    [Header("Offsets")]
    [SerializeField] private float _spawnPositonOffset_X = 0.1f;
    [SerializeField] private float _spawnPositonOffset_Y = 0.1f;
    [SerializeField] private float _shootDelayOffset = 0.1f;

    [Header("Sound")]
    [SerializeField] private List<AudioClip> _shootSounds = new List<AudioClip>();

    private float _PRJDamage;
    private float _shootDelay;
    private int _projectileCount;
    private float _lastShootTime;
    private bool _isShooting;
    private List<Transform> _shootP = new List<Transform>();
    private AudioSource _audioSource;

    private void Start()
    {
        if (_shootPoints == null)
            _shootPoints = transform;

        _lastShootTime = Time.time;
        ChangeShootPoints(_projectileCount);

        _audioSource = GetComponent<AudioSource>();
    }

    public void IsShooting(bool isShooting) { _isShooting  = isShooting; }

    public void TakeStats(float damage, float shootDelay, int projectileCount)
    {
        _PRJDamage = damage;
        _shootDelay = shootDelay;
        _projectileCount = projectileCount;
    }

    void Update()
    {
        if (!_isShooting) return;

        if (Time.time - _lastShootTime > _shootDelay && OnScreen())
        {
            foreach (var p in _shootP)
            {
                _lastShootTime = Time.time;
                _lastShootTime += Random.Range(-_shootDelayOffset, _shootDelayOffset);

                Vector3 InitPos = new Vector3(p.position.x + Random.Range(-_spawnPositonOffset_X, _spawnPositonOffset_X),
                    p.position.y + Random.Range(-_spawnPositonOffset_Y, _spawnPositonOffset_Y),
                    0);

                var projectile = Instantiate(_projectilePrefab, InitPos, Quaternion.identity);
                projectile.Initialize(_PRJDamage);
                projectile.GetComponent<ObjectMovement>().StartMove(InitPos);
            }
            if (_shootSounds.Count != 0)
                _audioSource.PlayOneShot(_shootSounds[Random.Range(0, _shootSounds.Count)]);

            if (_shootVFXPrefab != null)
                Instantiate(_shootVFXPrefab, _shootPoints.position, Quaternion.identity, transform);

            if (_shootFlashAnimator != null)
                _shootFlashAnimator.SetTrigger("Shoot");
        }
    }

    public void ChangeShootPoints(int count)
    {
        foreach (Transform p in _shootP)
            Destroy(p.gameObject);
        _shootP.Clear();

        if (count == 1)
        {
            GameObject bullet = new GameObject($"{gameObject.name}ShootPoint");
            bullet.transform.SetParent(_shootPoints);
            bullet.transform.localPosition = Vector3.zero;
            _shootP.Add(bullet.transform);
            return;
        }

        float totalWidth = count - 0.5f;
        float startX = -totalWidth / 2f;
        float step = totalWidth / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * step;
            GameObject bullet = new GameObject($"{gameObject.name}ShootPoint{i}");
            bullet.transform.SetParent(_shootPoints);
            bullet.transform.localPosition = new Vector3(x, 0, 0);
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
