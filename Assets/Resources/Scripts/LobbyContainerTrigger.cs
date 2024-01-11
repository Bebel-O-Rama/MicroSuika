using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class LobbyContainerTrigger : MonoBehaviour
{
    private Rigidbody2D _rb;
    public UnityEvent OnTrigger;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        SetTrigger(false);
    }

    public void SetTrigger(bool toEnable)
    {
        _rb.simulated = toEnable;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            OnTrigger?.Invoke();
        }
    }
}
