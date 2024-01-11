using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class LobbyContainerTrigger : MonoBehaviour
{
    // Trash code, but for now it'll get the job done to test a few things
    
    [SerializeField] public IntReference numberActivePlayer; 
    [SerializeField] private GameObject textEnoughPlayers;
    [SerializeField] private GameObject lidCollider;

    private int _previousPlayerNumber;
    
    private Collider2D collider;
    public UnityEvent OnTrigger;

    private void OnEnable()
    {
        _previousPlayerNumber = 0;
        collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (numberActivePlayer.Value != _previousPlayerNumber)
        {
            _previousPlayerNumber = numberActivePlayer.Value;
            lidCollider.SetActive(_previousPlayerNumber < 2);
            textEnoughPlayers.SetActive(_previousPlayerNumber >= 2);
            collider.enabled = _previousPlayerNumber >= 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
            OnTrigger?.Invoke();
        }
    }
}
