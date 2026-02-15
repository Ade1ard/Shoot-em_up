using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;

    [Header("Input")]
    [SerializeField] InputActionAsset _inputActions;

    private InputAction _moveAction;
    private InputAction _slowDownAction;

    private Vector2 _moveInput;
    private bool _slowDown;

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
        _slowDown = _slowDownAction.IsPressed();
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 targetVelocity = _moveInput * (_slowDown? _moveSpeed / 2 : _moveSpeed);

        Vector2 velocityDifference = targetVelocity - _rigidbody.linearVelocity;
        float accelerationRate = _moveInput.magnitude > 0.1f ? _acceleration : _deceleration;

        _rigidbody.AddForce(velocityDifference * accelerationRate * Time.fixedDeltaTime);
    }

    private void InitializeInput()
    {
        InputActionMap playerActionMap = _inputActions.FindActionMap("Player");

        _moveAction = playerActionMap.FindAction("Move");
        _slowDownAction = playerActionMap.FindAction("SlowDown");

        _moveAction.Enable();
        _slowDownAction.Enable();
    }
}
