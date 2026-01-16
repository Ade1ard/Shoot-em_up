using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class ProjectileCont : MonoBehaviour
{
    [SerializeField] private float _speed = 1000;

    private Rigidbody2D _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float speed)
    {
        if (speed != 0)
            _speed = speed;
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(Vector2.up * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ScreenBounds"))
            Destroy(gameObject);
    }
}