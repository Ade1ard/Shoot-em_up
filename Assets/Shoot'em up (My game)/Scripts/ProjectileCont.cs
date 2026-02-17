using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class ProjectileCont : MonoBehaviour
{
    [SerializeField] private float _damage = 10;

    public void Initialize(float damage = default)
    {
        if (damage != 0)
            _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable IDA) && OnScreen())
        {
            IDA.DealDamage(_damage);
            DOTween.Kill(transform);
            Destroy(gameObject);
        }
    }

    bool OnScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
            viewportPos.y >= 0 && viewportPos.y <= 1;
    }
}