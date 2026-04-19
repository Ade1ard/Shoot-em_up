using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IInitializable
{
    [Header("Input")]
    [SerializeField] InputActionAsset _inputActions;

    private InputActionMap _playerActionMap;
    private InputActionMap _uiActionMap;

    public Action<Vector2> OnMoveInputChanged;
    public Action<bool> OnSlowDownChanged;
    public Action OnGameRestarted;

    private InputAction _moveAction;
    private InputAction _slowDownAction;
    private InputAction _gameRestartedAction;

    public void Init()
    {
        InitializeInput();
    }

    private void MoveInputPerformed(InputAction.CallbackContext context)
    {
        OnMoveInputChanged?.Invoke(context.ReadValue<Vector2>());
    }

    private void MoveInputCanceled(InputAction.CallbackContext context)
    {
        OnMoveInputChanged?.Invoke(Vector2.zero);
    }

    private void SlowDownPerformed(InputAction.CallbackContext context)
    {
        OnSlowDownChanged?.Invoke(true);
    }

    private void SlowDownCanceled(InputAction.CallbackContext context)
    {
        OnSlowDownChanged?.Invoke(false);
    }

    private void OnGameRestartPerformed(InputAction.CallbackContext context)
    {
        OnGameRestarted?.Invoke();
    }

    public void EnablePlayerInput(bool enable)
    {
        if (enable)
            _playerActionMap.Enable();
        else
            _playerActionMap.Disable();
    }

    public void RestartEnable(bool enable)
    {
        if (enable)
            _gameRestartedAction.Enable();
        else
            _gameRestartedAction.Disable();
    }

    private void InitializeInput()
    {
        _playerActionMap = _inputActions.FindActionMap("Player");
        _uiActionMap = _inputActions.FindActionMap("UI");

        _moveAction = _playerActionMap.FindAction("Move");
        _slowDownAction = _playerActionMap.FindAction("SlowDown");

        _gameRestartedAction = _uiActionMap.FindAction("Restart");

        _moveAction.performed += MoveInputPerformed;
        _moveAction.canceled += MoveInputCanceled;

        _slowDownAction.performed += SlowDownPerformed;
        _slowDownAction.canceled += SlowDownCanceled;

        _gameRestartedAction.performed += OnGameRestartPerformed;
    }

    private void OnDisable()
    {
        _moveAction.performed -= MoveInputPerformed;
        _moveAction.canceled -= MoveInputCanceled;

        _slowDownAction.performed -= SlowDownPerformed;
        _slowDownAction.canceled -= SlowDownCanceled;

        _gameRestartedAction.performed -= OnGameRestartPerformed;
    }
}
