using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player/Player Game Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player number")]
    [Range(0, 3)] public int playerNumber;
    
    [Header(("Input Parameters"))]
    public InputDevice inputDevice;
    public InputUser inputUser;
    private bool _isConnected = false;
    
    [Header("Score References")]
    public IntReference mainScore;
    public IntReference miniGameScore;
    
    public bool IsPlayerConnected() => _isConnected;
    
    public void SetInputParameters(InputDevice device, InputUser user)
    {
        inputDevice = device;
        inputUser = user;
        _isConnected = true;
    }

    public void ResetInputParameters() => _isConnected = false;

    public void ResetMainScore() => mainScore.Variable.SetValue(0);
    public void ResetMiniGameScore() => miniGameScore.Variable.SetValue(0);

    private void OnEnable()
    {
        _isConnected = false;
    }
}
