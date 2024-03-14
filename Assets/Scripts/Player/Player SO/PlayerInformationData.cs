using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiSuika.Player
{
    [CreateAssetMenu(menuName = "Player/Player Information Data")]
    public class PlayerInformationData : ScriptableObject
    {
        private Stack<PlayerInformation> _playersInformation;
        private IntReference _numberOfActivePlayer;

        private void OnEnable()
        {
            _playersInformation ??= new Stack<PlayerInformation>();
            _numberOfActivePlayer = new IntReference
                { UseConstant = false, Variable = CreateInstance<IntVariable>() };
            UpdateNumberOfActivePlayer();
        }

        public int PushPlayerInformation(InputDevice inputDevice)
        {
            var playerIndex = _playersInformation.Count;
            var newPlayerInformation = new PlayerInformation(playerIndex, inputDevice);

            _playersInformation.Push(newPlayerInformation);
            UpdateNumberOfActivePlayer();

            return newPlayerInformation.GetPlayerIndexReference();
        }

        public int PopPlayerInformation()
        {
            var playerInfo = _playersInformation.Pop();
            UpdateNumberOfActivePlayer();
            return playerInfo.GetPlayerIndexReference();
        }


        public IntReference GetNumberOfActivePlayer() => _numberOfActivePlayer;
        public InputDevice PeekInputDevice() => _playersInformation.Peek().GetInputDevice();

        public List<InputDevice> GetOrderedInputDevices()
        {
            if (_playersInformation == null || _playersInformation.Count == 0)
            {
                Debug.LogError(
                    "You're trying to GetPlayersInformation() while the _playerInformation is not yet initialized or empty");
                return null;
            }

            return _playersInformation.OrderByDescending(pi => pi.GetPlayerIndexReference()).Select(pi => pi.GetInputDevice())
                .ToList();
        }

        private void UpdateNumberOfActivePlayer() => _numberOfActivePlayer.Variable.SetValue(_playersInformation.Count);
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