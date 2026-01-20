using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class ProjectileCont : MonoBehaviour
{
    [SerializeField] private float _speed = 1000;
    [SerializeField] private float _damage = 10;

    private Rigidbody2D _rigidbody;

    public void Initialize(float speed = default, float damage = default)
    {
        if (speed != 0)
            _speed = speed;
        if (damage != 0)
            _damage = damage;

        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = (Vector2.up * _speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ScreenBounds"))
            Destroy(gameObject, 1);
    }
}