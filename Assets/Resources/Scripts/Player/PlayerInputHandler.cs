using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("The vertical input of the Joystick needs to have a big deadzone, otherwise the player might switch target by accident")]
    [SerializeField] [Range(0f, 1f)] private float discreteAxistDeadzone = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float continuousAxisDeadzone = 0.1f;

    [Header("InputEvent")] 
    public UnityEvent<float> OnHorizontalMvtContinuous;
    public UnityEvent<int> OnHorizontalMvtDiscrete;
    public UnityEvent<float> OnVerticalMvtContinuous;
    public UnityEvent<int> OnVerticalMvtDiscrete;
    public UnityEvent OnShoot;
    public UnityEvent OnPause;

    [Header("Debug and Testing Parameters")]
    [Tooltip("It'll change the feel of the movements for players with a controllers. Might be good or bad")]
    [SerializeField] private bool showInputLog = false;
    
    // PlayerInput Parameters
    private PlayerInput _playerInput;
    private int _playerInputIndex;
    private string _playerInputDeviceName;
    private bool useHorizontalContinuous = true;
    private bool useHorizontalDiscrete = true;
    private bool useVerticalContinuous = true;
    private bool useVerticalDiscrete = true;
    
    // Horizontal Inputs Parameters
    private float _xAxis;
    private int _latestNaturalXAxis;

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

        if (OnHorizontalMvtContinuous.GetPersistentEventCount() == 0) 
            useHorizontalContinuous = false;
        if (OnHorizontalMvtDiscrete.GetPersistentEventCount() == 0) 
            useHorizontalDiscrete = false;
        if (OnVerticalMvtContinuous.GetPersistentEventCount() == 0) 
            useVerticalContinuous = false;
        if (OnVerticalMvtDiscrete.GetPersistentEventCount() == 0) 
            useVerticalDiscrete = false;
    }

    private void Update()
    {
        if (useHorizontalContinuous)
            ProcessContinuousAxis(_xAxis, OnHorizontalMvtContinuous);
        if (useHorizontalDiscrete)
            ProcessDiscreteAxis(_xAxis, true, OnHorizontalMvtDiscrete);
        if (useVerticalContinuous)
            ProcessContinuousAxis(_yAxis, OnVerticalMvtContinuous);
        if (useVerticalDiscrete)
            ProcessDiscreteAxis(_yAxis, false, OnVerticalMvtDiscrete);
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

    private void ProcessContinuousAxis(float axis, UnityEvent<float> inputEvent)
    {
        if (Mathf.Abs(axis) < continuousAxisDeadzone)
            return;

        inputEvent?.Invoke(axis);
        if (showInputLog)
            Debug.Log($"P{_playerInputIndex}, {inputEvent} by {axis} (continuous)");
    }

    private void ProcessDiscreteAxis(float axis, bool isHorizontal, UnityEvent<int> inputEvent)
    {
        int currentValue = Mathf.Abs(axis) < discreteAxistDeadzone ? 0 : (axis > 0 ? 1 : -1);
        if (isHorizontal)
        {
            if (_latestNaturalXAxis == currentValue)
                return;
            _latestNaturalXAxis = currentValue;
        }
        else
        {
            if (_latestNaturalYAxis == currentValue)
                return;
            _latestNaturalYAxis = currentValue;
        }

        if (currentValue != 0)
        {
            inputEvent?.Invoke(currentValue);
            if (showInputLog)
                Debug.Log($"P{_playerInputIndex}, by {currentValue} (discrete)");
        }
    }
}