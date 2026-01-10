using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;

    [Header("Input")]
    [SerializeField] InputActionAsset _inputActions;

    private InputAction _moveAction;

    private Vector2 _moveInput;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        InitializeInput();
    }

    private void Update()
    {
        _animator.SetFloat("HorizontalDirection", _moveAction.ReadValue<Vector2>().x);
    }

    private void FixedUpdate()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 targetVelocity = _moveInput * _moveSpeed;

        Vector2 velocityDifference = targetVelocity - _rigidbody.linearVelocity;
        float accelerationRate = _moveInput.magnitude > 0.1f ? _acceleration : _deceleration;

        _rigidbody.AddForce(velocityDifference * accelerationRate * Time.fixedDeltaTime);

        //if (_rigidbody.linearVelocity.magnitude > _moveSpeed)
        //    _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _moveSpeed;
    }

    private void InitializeInput()
    {
        InputActionMap playerActionMap = _inputActions.FindActionMap("Player");

        _moveAction = playerActionMap.FindAction("Move");

        _moveAction.Enable();
    }
}
