using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour, IInitializable
{
    [Header("Movement settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;

    private Vector2 _moveInput;
    private bool _slowDown;

    private bool _isInputEnabled;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private InputManager _inputManager;

    private int _animatorKey;

    public void Init()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _animatorKey = Animator.StringToHash("HorizontalDirection");
        _inputManager = G.Get<InputManager>();

        _inputManager.OnMoveInputChanged += MoveInputChanged;
        _inputManager.OnSlowDownChanged += SlowDownChanged;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 targetVelocity = _moveInput * (_slowDown? _moveSpeed / 2 : _moveSpeed);

        Vector2 velocityDifference = targetVelocity - _rigidbody.linearVelocity;
        float accelerationRate = _moveInput.magnitude > 0.1f ? _acceleration : _deceleration;

        _rigidbody.AddForce(velocityDifference * accelerationRate * Time.fixedDeltaTime);
    }

    private void MoveInputChanged(Vector2 vec)
    {
        if (_isInputEnabled)
        {
            _moveInput = vec;
            _animator.SetFloat(_animatorKey, vec.x);
        }
    }

    private void SlowDownChanged(bool enable)
    {
        if (_isInputEnabled)
            _slowDown = enable;
    }

    public void SetInputEnabled(bool enabled)
    {
        _isInputEnabled = enabled;

        if (!enabled)
        {
            _moveInput = Vector2.zero;
            _animator.SetFloat(_animatorKey, 0f);
        }
    }

    private void OnDisable()
    {
        _inputManager.OnMoveInputChanged -= MoveInputChanged;
        _inputManager.OnSlowDownChanged -= SlowDownChanged;
    }
}
