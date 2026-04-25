using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class ProjectileCont : MonoBehaviour
{
    [SerializeField] private ParticleSystem _clearVFX;
    private float _damage;

    public void Initialize(float damage = default)
    {
        if (damage != 0)
            _damage = damage;
        else
            _damage = 10;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable ID) && OnScreen())
        {
            if (!ID.CanDamage()) return;

            ID.DealDamage(_damage, collision.ClosestPoint(transform.position));

            DOTween.Kill(transform);
            Destroy(gameObject);
        }
    }

    public void Clear()
    {
        if (_clearVFX != null)
            Instantiate(_clearVFX, transform.position, Quaternion.identity);

        DOTween.Kill(transform);
        Destroy(gameObject);
    }

    bool OnScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
            viewportPos.y >= 0 && viewportPos.y <= 1;
    }
}