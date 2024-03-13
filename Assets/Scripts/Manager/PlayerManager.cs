using System;
using System.Diagnostics.CodeAnalysis;
using MultiSuika.Player;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInputManager = UnityEngine.InputSystem.PlayerInputManager;

namespace MultiSuika.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton

        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static PlayerManager Instance => _instance ??= new PlayerManager();

        private static PlayerManager _instance;

        private PlayerManager()
        {
        }

        #endregion

        [Header("Players Information")]
        [SerializeField] private PlayerInformationData _playerInformationData;
        
        [Header("Input Connexions")]
        [SerializeField] [Min(1)] private int _maximumNumberOfPlayer = 4;
        [SerializeField] private PlayerInputManager _inputManagerPrefab;

        private PlayerInputManager _playerInputManager;
        private bool _isJoiningEnabled = false;
        private Action<int, PlayerInput> _onAddNewPlayer;
        private Action<int> _onRemovePlayer;

        private void Awake()
        {
            _instance = this;
        }

        public void SetJoiningEnabled(bool isEnabled)
        {
            if (_isJoiningEnabled == isEnabled) 
                return;
            _isJoiningEnabled = isEnabled;
            if (_isJoiningEnabled)
            {
                _playerInputManager ??= Instantiate(_inputManagerPrefab, transform);
                _playerInputManager.playerJoinedEvent.AddListener(AddNewPlayer);
            }
            else if (_playerInputManager != null)
            {
                Destroy(_playerInputManager);
            }
            UpdatePlayerJoining();
        }

        public void AddNewPlayer(PlayerInput inputDevice)
        {
            var playerIndex = _playerInformationData.AddNewPlayerData(inputDevice.devices[0]);
            UpdatePlayerJoining();
            _onAddNewPlayer?.Invoke(playerIndex, inputDevice);
        }
        
        public void RemovePlayer(int playerIndex)
        {
            _playerInformationData.ClearPlayerInformation(playerIndex);
            UpdatePlayerJoining();
            _onRemovePlayer?.Invoke(playerIndex);
        }

        public void ClearAllPlayers()
        {
            for (int i = GetNumberActivePlayerRef() - 1; i >= 0; i--)
                RemovePlayer(i);
        }

        // TODO: Could be cleaner
        private void UpdatePlayerJoining()
        {
            if (!_isJoiningEnabled)
                return;
            
            if (GetNumberActivePlayerRef() < _maximumNumberOfPlayer)
                _playerInputManager.EnableJoining();
            else if (GetNumberActivePlayerRef() >= _maximumNumberOfPlayer)
                _playerInputManager.DisableJoining();
        }
        
        
        public void GetPlayerInputDevice(int playerIndex) => _playerInformationData.GetInputDevice(playerIndex);
        public IntReference GetNumberActivePlayerRef() => _playerInformationData.GetNumberActivePlayerRef();
        
        
        public void SubscribeAddNewPlayer(Action<int, PlayerInput> method) => _onAddNewPlayer += method;
        public void UnsubscribeAddNewPlayer(Action<int, PlayerInput> method) => _onAddNewPlayer -= method;
        public void SubscribeRemovePlayer(Action<int> method) => _onRemovePlayer += method;
        public void UnsubscribeRemovePlayer(Action<int> method) => _onRemovePlayer -= method;
    }
}