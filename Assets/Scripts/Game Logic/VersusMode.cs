using System.Collections.Generic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Game_Logic
{
    public class VersusMode : MonoBehaviour
    {
        [SerializeField] public GameData gameData;
        [SerializeField] public GameModeData gameModeData;
    
        private int _numberPlayerConnected;
        private List<Player.Player> _players;
        private List<Cannon.Cannon> _cannons;
        private List<Container.Container> _containers;

        private List<int> _scoreSave;
    
        [Header("DEBUG DEBUG DEBUG")] public bool useDebugSpawnContainer = false;
        [Range(0, 4)][SerializeField] public int debugFakeNumberCount = 2;
    
        private void Awake()
        {
            _numberPlayerConnected = gameData.GetConnectedPlayerQuantity();
        
            // TODO: REMOVE THIS TEMP LINE (fake the player count)
            _numberPlayerConnected = useDebugSpawnContainer ? debugFakeNumberCount : _numberPlayerConnected;
        
            //// Init and set containers
            _containers = Initializer.InstantiateContainers(_numberPlayerConnected, gameModeData);
            Initializer.SetContainersParameters(_containers, gameModeData);
        
            //// Init and set cannons
            _cannons = Initializer.InstantiateCannons(_numberPlayerConnected, gameModeData,
                _containers);
            Initializer.SetCannonsParameters(_cannons, _containers, gameModeData, gameData.playerDataList);
        
            //// Init and set players
            _players = Initializer.InstantiatePlayers(gameData.GetConnectedPlayersData(), gameModeData);
            Initializer.SetPlayersParameters(gameData.playerDataList, _players);
            Initializer.ConnectCannonsToPlayers(_cannons, _players, true);

        }
    }
}
