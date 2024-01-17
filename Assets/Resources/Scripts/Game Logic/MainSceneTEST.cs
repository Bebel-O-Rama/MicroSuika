using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MainSceneTEST : MonoBehaviour
{
    [SerializeField] public GameData gameData;
    [SerializeField] public GameObject playerPf;

    public List<Player> players;
    
    private void OnEnable()
    {
        gameData.InstantiatePlayers(gameData.lobby);
    }
}
