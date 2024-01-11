using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    private PlayerInputManager _playerInputManager;

    public static GameManager Instance => _instance ??= new GameManager();
    private static GameManager _instance;

    private GameManager()
    {
    }

    private void Awake()
    {
        _instance = this;
    }
}
