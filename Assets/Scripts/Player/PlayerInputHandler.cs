using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb2d;
    [SerializeField] public float speed;
    private Player player;
    private PlayerInput playerInput;
    private int playerInputIndex;
    [SerializeField] public float xAxis;
    private bool isMoving = false;

    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInputIndex = playerInput.playerIndex;
        Debug.Log($"NEW PLAYER!!! : P{playerInputIndex}({playerInput.devices[0].displayName}) joined the game!");
    }

    private void Update()
    {
        // if (isMoving == false && xAxis != 0f)
        // {
        //     Debug.Log("START MOVING");
        //     isMoving = true;
        // }
        //
        // if (isMoving == true && xAxis is > -0.01f and < 0.01f)
        // {
        //     Debug.Log("STOOOOOP");
        //     isMoving = false;
        // }
        transform.parent.transform.Translate(xAxis * Time.deltaTime * speed, 0, 0);

    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if(context.performed)
            Debug.Log($"P{playerInputIndex} dropped a ball");
    }
    
    public void OnHorizontalMvt(InputAction.CallbackContext context)
    {
            xAxis = context.ReadValue<float>();
        if (context.performed)
        {
            // Debug.Log($"P{playerInputIndex} strafed by {context.ReadValue<float>()}");
        }
    }
    
    public void OnVerticalMvt(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log($"P{playerInputIndex} moved on the y axis by {context.ReadValue<float>()}");
            transform.parent.transform.Translate(0, context.ReadValue<float>() * Time.deltaTime * speed * 10, 0);

        }
    }
    
    public void OnPause(InputAction.CallbackContext context)
    {
        if(context.performed)
            Debug.Log($"P{playerInputIndex} paused the game");
    }
    
    public void OnDeviceLost(PlayerInput playerInput)
    {
        Debug.Log($"P{playerInputIndex} just disconnected");
    }
    
    public void OnDeviceRegained(PlayerInput playerInput)
    {
        Debug.Log($"P{playerInputIndex} reconnected");
    }
    
    public void OnControlsChanged(PlayerInput playerInput)
    {
        Debug.Log($"The controls of P{playerInputIndex} have changed");
    }
}
