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
        if (collision.TryGetComponent<IDamageable>(out IDamageable IDA))
        {
            IDA.DealDamage(_damage);
            DOTween.Kill(transform);
            Destroy(gameObject);
        }
    }
}