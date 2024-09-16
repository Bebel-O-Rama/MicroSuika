using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Player;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using PlayerInputManager = UnityEngine.InputSystem.PlayerInputManager;

namespace MultiSuika.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton

        public static PlayerManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);

            Initialize();
        }

        #endregion

        [Header("Players Information")] 
        [SerializeField] [Min(1)] private int _maximumNumberOfPlayers = 4;
        [SerializeField] private PlayerManagerData _playerManagerData;

        [Header("Input System")] 
        [SerializeField] private PlayerInputManager _inputManagerPrefab;
        [SerializeField] private GameObject _inputHandlerPrefab;

        [Header("Debug/Fallback")] 
        [SerializeField] private bool _isFallbackEnabled = false;
        [SerializeField] [Range(1, 4)] private int _numberPlayerFallback = 2;
        
        private Action<int, PlayerInput> _onPlayerPush;
        private Action<int> _onPlayerPop;

        private Stack<PlayerInputHandler> _playerInputHandler;
        private PlayerInputManager _playerInputManager;
        private bool _isJoiningEnabled = false;

        private void Initialize()
        {
            _playerManagerData.Initialize();
            
            // Spawn players who were registered in the previous scene
            _playerInputHandler = new Stack<PlayerInputHandler>();
            var numberOfActivePlayer = GetNumberOfActivePlayer();
            
            // TEMPORARY DEBUG MODE
            // It should be either removed or cleaned up, especially when merging the lobby with this scene
            if (_isFallbackEnabled && numberOfActivePlayer <= 0)
            {
                for (int i = 0; i < _numberPlayerFallback; i++)
                {
                    _playerManagerData.PushPlayerInformation(Keyboard.current);
                    var playerInputObj = PlayerInput.Instantiate(_inputHandlerPrefab, i, pairWithDevice: Keyboard.current);
                    PushPlayerInputHandler(playerInputObj);
                }
                return;
            }
            
            if (numberOfActivePlayer <= 0)
                return;
            var inputDevices = _playerManagerData.GetOrderedInputDevices();
            for (int i = 0; i < inputDevices.Count; i++)
            {
                var playerInputObj = PlayerInput.Instantiate(_inputHandlerPrefab, i, pairWithDevice: inputDevices[i]);
                PushPlayerInputHandler(playerInputObj);
            }
        }

        public void SetJoiningEnabled(bool isEnabled)
        {
            if (_isJoiningEnabled == isEnabled)
                return;
            _isJoiningEnabled = isEnabled;
            if (_isJoiningEnabled)
            {
                _playerInputManager ??= Instantiate(_inputManagerPrefab, transform);
                _playerInputManager.playerJoinedEvent.AddListener(PushPlayer);
            }
            else if (_playerInputManager != null)
            {
                Destroy(_playerInputManager);
            }

            UpdatePlayerJoining();
        }

        private void PushPlayer(PlayerInput playerInput)
        {
            var playerIndex = _playerManagerData.PushPlayerInformation(playerInput.devices[0]);
            PushPlayerInputHandler(playerInput);
            UpdatePlayerJoining();
            _onPlayerPush?.Invoke(playerIndex, playerInput);
        }

        private void PopPlayer()
        {
            var playerIndex = _playerManagerData.PopPlayerInformation();
            PopPlayerInputHandler();
            UpdatePlayerJoining();
            _onPlayerPop?.Invoke(playerIndex);
        }

        public void ClearAllPlayers()
        {
            while (GetNumberOfActivePlayer() > 0)
                PopPlayer();
        }

        private void PushPlayerInputHandler(PlayerInput playerInput = null)
        {
            if (playerInput)
            {
                _playerInputHandler.Push(playerInput.GetComponent<PlayerInputHandler>() ??
                                         InstantiatePlayerInputHandler());
                return;
            }

            InstantiatePlayerInputHandler();
        }

        private void PopPlayerInputHandler()
        {
            var handler = _playerInputHandler.Pop();
            Destroy(handler.gameObject);
        }

        private void UpdatePlayerJoining()
        {
            if (!_isJoiningEnabled)
                return;

            if (GetNumberOfActivePlayer() < _maximumNumberOfPlayers)
                _playerInputManager.EnableJoining();
            else
                _playerInputManager.DisableJoining();
        }

        private PlayerInputHandler InstantiatePlayerInputHandler()
        {
            var playerIndex = _playerInputHandler.Count - 1;
            if (playerIndex != GetNumberOfActivePlayer() - 1)
                Debug.LogError(
                    "Trying to spawn a PlayerInputHandler, but the playerIndex doesn't match with the index of the player added last.");
            var playerInputObj = PlayerInput.Instantiate(_inputHandlerPrefab, playerIndex,
                pairWithDevice: _playerManagerData.PeekInputDevice());
            return playerInputObj.GetComponentInParent<PlayerInputHandler>();
        }

        #region Getter

        public PlayerInputHandler GetPlayerInputHandler(int playerIndex = -1)
        {
            if (playerIndex < 0 || playerIndex >= _playerInputHandler.Count)
                return _playerInputHandler.Peek();
            return _playerInputHandler.ElementAtOrDefault(playerIndex);
        }

        public IntReference GetNumberOfActivePlayer() => _playerManagerData.GetNumberOfActivePlayer();

        #endregion

        #region Delegate

        public void SubscribePlayerPush(Action<int, PlayerInput> method) => _onPlayerPush += method;
        public void UnsubscribePlayerPush(Action<int, PlayerInput> method) => _onPlayerPush -= method;
        public void SubscribePlayerPop(Action<int> method) => _onPlayerPop += method;
        public void UnsubscribePlayerPop(Action<int> method) => _onPlayerPop -= method;

        #endregion
    }
}