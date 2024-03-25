using System.Collections.Generic;
using System.Linq;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiSuika.Player
{
    [CreateAssetMenu(menuName = "Player/Player Manager Data")]
    public class PlayerManagerData : ScriptableObject
    {
        private Stack<PlayerInformation> _playersInformation;
        private IntReference _numberOfActivePlayer;

        public void Initialize()
        {
            _playersInformation ??= new Stack<PlayerInformation>();
            _numberOfActivePlayer = new IntReference();
            UpdateNumberOfActivePlayer();
        }

        public int PushPlayerInformation(InputDevice inputDevice)
        {
            var playerIndex = _playersInformation.Count;
            var newPlayerInformation = new PlayerInformation(playerIndex, inputDevice);

            _playersInformation.Push(newPlayerInformation);
            UpdateNumberOfActivePlayer();

            return newPlayerInformation.PlayerIndex;
        }

        public int PopPlayerInformation()
        {
            var playerInfo = _playersInformation.Pop();
            UpdateNumberOfActivePlayer();
            return playerInfo.PlayerIndex;
        }

        public IntReference GetNumberOfActivePlayer() => _numberOfActivePlayer;
        public InputDevice PeekInputDevice() => _playersInformation.Peek().InputDevice;

        public List<InputDevice> GetOrderedInputDevices()
        {
            return _playersInformation.OrderByDescending(pi => pi.PlayerIndex)
                .Select(pi => pi.InputDevice)
                .ToList();
        }

        private void UpdateNumberOfActivePlayer() => _numberOfActivePlayer.Variable.SetValue(_playersInformation.Count);
    }

    public class PlayerInformation
    {
        public int PlayerIndex { get; }
        public InputDevice InputDevice { get; }

        public PlayerInformation(int playerIndex, InputDevice inputDevice)
        {
            PlayerIndex = playerIndex;
            InputDevice = inputDevice;
        }
    }
}