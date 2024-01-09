using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("The vertical input of the Joystick needs to have a big deadzone, otherwise the player might switch target by accident")]
    [SerializeField] [Range(0f, 1f)] private float verticalInputDeadzone = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float horizontalInputDeadzone = 0.1f;
    
    [Header("InputEvent")] public UnityEvent<float> OnHorizontalMvt;
    public UnityEvent<int> OnVerticalMvt;
    public UnityEvent OnShoot;
    public UnityEvent OnPause;

    [Header("Debug and Testing Parameters")]
    [Tooltip("It'll change the feel of the movements for players with a controllers. Might be good or bad")]
    [SerializeField] private bool useFloatforHorizontalInput = false;
    [SerializeField] private bool showInputLog = false;
    
    // PlayerInput Parameters
    private PlayerInput _playerInput;
    private int _playerInputIndex;
    private string _playerInputDeviceName;

    // Horizontal Inputs Parameters
    private float _xAxis;
    private bool _isHorizontalInDeadzone = true;

    // Vertical Inputs Parameters
    private float _yAxis;
    private int _latestNaturalYAxis;

    private void OnEnable()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInputIndex = _playerInput.playerIndex;
        _playerInputDeviceName = _playerInput.devices[0].displayName;
        if (showInputLog)
            Debug.Log($"NEW PLAYER!!! : P{_playerInputIndex}({_playerInputDeviceName}) joined the game!");
    }

    private void Update()
    {
        ProcessHorizontalInputs();
        ProcessVerticalInputs();
    }
    
    public void OnHorizontalMvtInput(InputAction.CallbackContext context)
    {
        _xAxis = context.ReadValue<float>();
    }

    public void OnVerticalMvtInput(InputAction.CallbackContext context)
    {
        _yAxis = context.ReadValue<float>();
    }

    public void OnShootInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnShoot?.Invoke();
            if (showInputLog)
                Debug.Log($"P{_playerInputIndex} dropped a ball");
        }
    }
    
    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnPause?.Invoke();
            if (showInputLog)
                Debug.Log($"P{_playerInputIndex} paused the game");
        }
    }

    public void OnDeviceLost(PlayerInput playerInput)
    {
        if (showInputLog)
            Debug.Log($"P{_playerInputIndex} just disconnected");
    }

    public void OnDeviceRegained(PlayerInput playerInput)
    {
        if (showInputLog)
            Debug.Log($"P{_playerInputIndex} reconnected");
    }

    public void OnControlsChanged(PlayerInput playerInput)
    {
        if (showInputLog)
            Debug.Log($"The controls of P{_playerInputIndex} have changed");
    }

    private void ProcessHorizontalInputs()
    {
        if (Mathf.Abs(_xAxis) < horizontalInputDeadzone)
            return;
        var currentXValue = useFloatforHorizontalInput ? _xAxis : (_xAxis > 0 ? 1f : -1f);

        OnHorizontalMvt?.Invoke(currentXValue);
        if (showInputLog)
            Debug.Log($"P{_playerInputIndex} strafed by {_xAxis}");
    }

    private void ProcessVerticalInputs()
    {
        int currentNaturalYValue = Mathf.Abs(_yAxis) < verticalInputDeadzone ? 0 : (_yAxis > 0 ? 1 : -1);

        if (_latestNaturalYAxis == currentNaturalYValue)
            return;
        _latestNaturalYAxis = currentNaturalYValue;

        if (_latestNaturalYAxis != 0)
        {
            OnVerticalMvt?.Invoke(_latestNaturalYAxis);
            if (showInputLog)
                Debug.Log($"P{_playerInputIndex} moved on the y axis by {_latestNaturalYAxis}");
        }
    }
}