using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiSuika.Player
{
    [CreateAssetMenu(menuName = "Player/Player Game Data (OLD)")]
    public class PlayerDataOLD : ScriptableObject
    {
        [Header("Player number")]
        [Range(0, 3)] public int playerIndexNumber;
    
        [Header(("Input Parameters"))]
        public InputDevice inputDevice;
        private bool _isConnected = false;
    
        [Header("Score Reference")]
        public IntReference mainScore;
    
        public bool IsPlayerConnected() => _isConnected;
    
        public void SetInputParameters(InputDevice device)
        {
            inputDevice = device;
            _isConnected = true;
        }

        public void ResetInputParameters() => _isConnected = false;

        public void ResetMainScore() => mainScore.Variable.SetValue(0);

        private void OnEnable()
        {
            _isConnected = false;
        }
    }
}
