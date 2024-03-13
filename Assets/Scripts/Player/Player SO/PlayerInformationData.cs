using System;
using System.Collections;
using System.Collections.Generic;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiSuika.Player
{
    [CreateAssetMenu(menuName = "Player/Player Information Data")]
    public class PlayerInformationData : ScriptableObject
    {
        private List<PlayerInformation> _playersInformation;
        private IntReference _numberActivePlayer;

        private void OnEnable()
        {
            _playersInformation ??= new List<PlayerInformation>();
            _numberActivePlayer = new IntReference
                { UseConstant = false, Variable = CreateInstance<IntVariable>() };
            UpdateNumberActivePlayer();
        }

        public int AddNewPlayerData(InputDevice inputDevice)
        {
            var newPlayerIndex = _playersInformation.Count;
            var newPlayerInformation = new PlayerInformation(newPlayerIndex, inputDevice);
            
            _playersInformation.Add(newPlayerInformation);
            UpdateNumberActivePlayer();
            
            return newPlayerInformation.GetPlayerIndexReference();
        }
        
        public void ClearPlayerInformation(int playerIndex)
        {
            _playersInformation.RemoveAt(playerIndex);
            UpdateNumberActivePlayer();
        }
        

        public IntReference GetNumberActivePlayerRef() => _numberActivePlayer;
        public InputDevice GetInputDevice(int playerIndex) => _playersInformation[playerIndex].GetInputDevice();
        private void UpdateNumberActivePlayer() => _numberActivePlayer.Variable.SetValue(_playersInformation.Count);
    }

    public class PlayerInformation
    {
        private readonly int _playerIndex;
        private readonly InputDevice _inputDevice;
        
        public PlayerInformation(int playerIndex, InputDevice inputDevice)
        {
            _playerIndex = playerIndex;
            _inputDevice = inputDevice;
        }
        
        public int GetPlayerIndexReference() => _playerIndex;
        public InputDevice GetInputDevice() => _inputDevice;
    }
}