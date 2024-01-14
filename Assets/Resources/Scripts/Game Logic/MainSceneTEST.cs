using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MainSceneTEST : MonoBehaviour
{
    [SerializeField] public GameData gameData;

    [SerializeField] public GameObject containerTwoPlayers;
    [SerializeField] public GameObject containerThreePlayers;
    [SerializeField] public GameObject containerFourPlayers;
    
    private int _numberPlayerConnected;
    
    private void OnEnable()
    {
        _numberPlayerConnected = gameData.GetCurrentPlayerQuantity();
        
        gameData.InstantiatePlayers(gameData.mainGame, _numberPlayerConnected);
        
        switch (_numberPlayerConnected)
        {
            case 2:
                containerTwoPlayers.SetActive(true);
                break;
            case 3:
                containerThreePlayers.SetActive(true);
                break;
            case 4:
                containerFourPlayers.SetActive(true);
                break;
        }
    }
}
