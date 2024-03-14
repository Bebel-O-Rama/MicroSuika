using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiSuika.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("The vertical input of the Joystick needs to have a big deadzone, otherwise the player might switch target by accident")]
        [SerializeField] [Range(0f, 1f)] private float _discreteAxisDeadzone = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float _continuousAxisDeadzone = 0.1f;
    
        public OnHorizontalMvtContinuousDelegate onHorizontalMvtContinuous;
        public OnHorizontalMvtDiscreteDelegate onHorizontalMvtDiscrete;
        public OnVerticalMvtContinuousDelegate onVerticalMvtContinuous;
        public OnVerticalMvtDiscreteDelegate onVerticalMvtDiscrete;
        public OnShootDelegate onShoot;
        public OnPauseDelegate onPause;

        public delegate void OnHorizontalMvtContinuousDelegate(float axis);
        public delegate void OnHorizontalMvtDiscreteDelegate(int axis);
        public delegate void OnVerticalMvtContinuousDelegate(float axis);
        public delegate void OnVerticalMvtDiscreteDelegate(int axis);
        public delegate void OnShootDelegate();
        public delegate void OnPauseDelegate();

        [Header("Debug and Testing Parameters")]
        [Tooltip("It'll change the feel of the movements for players with a controllers. Might be good or bad")]
        [SerializeField] private bool showInputLog = false;
    
        // PlayerInput Parameters
        private int _playerInputIndex;
        private string _playerInputDeviceName;

        // Horizontal Inputs Parameters
        private float _xAxis;
        private int _latestNaturalXAxis;

        // Vertical Inputs Parameters
        private float _yAxis;
        private int _latestNaturalYAxis;

        private void OnEnable()
        {
            // _playerInput = GetComponent<PlayerInput>();
            // _playerInputIndex = _playerInput.playerIndex;
            // _playerInputDeviceName = _playerInput.devices[0].displayName;
            if (showInputLog)
                Debug.Log($"NEW PLAYER!!! : P{_playerInputIndex}({_playerInputDeviceName}) joined the game!");
        }

        private void Update()
        {
            ProcessContinuousAxis(_xAxis, true);
            ProcessDiscreteAxis(_xAxis, true);
            ProcessContinuousAxis(_yAxis, false);
            ProcessDiscreteAxis(_yAxis, false);
        }
    
        public void OnHorizontalMvtInput(InputAction.CallbackContext context)
        {
            _xAxis = context.ReadValue<float>();
        }

        public void OnVerticalMvtInput(InputAction.CallbackContext context)
        {
            _yAxis = context.ReadValue<float>();
        }

        public void OnShootInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                onShoot?.Invoke();
                if (showInputLog)
                    Debug.Log($"P{_playerInputIndex} dropped a ball");
            }
        }
    
        public void OnPauseInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                onPause?.Invoke();
                if (showInputLog)
                    Debug.Log($"P{_playerInputIndex} paused the game");
            }
        }

        public void OnDeviceLost(PlayerInput playerInput)
        {
            if (showInputLog)
                Debug.Log($"P{_playerInputIndex} just disconnected");
        }

        public void OnDeviceRegained(PlayerInput playerInput)
        {
            if (showInputLog)
                Debug.Log($"P{_playerInputIndex} reconnected");
        }

        public void OnControlsChanged(PlayerInput playerInput)
        {
            if (showInputLog)
                Debug.Log($"The controls of P{_playerInputIndex} have changed");
        }

        private void ProcessContinuousAxis(float axis, bool isHorizontal)
        {
            if (Mathf.Abs(axis) < _continuousAxisDeadzone)
                return;
            if (isHorizontal)
                onHorizontalMvtContinuous?.Invoke(axis);
            else
            {
                onVerticalMvtContinuous?.Invoke(axis);
            }
        }

        private void ProcessDiscreteAxis(float axis, bool isHorizontal)
        {
            int currentValue = Mathf.Abs(axis) < _discreteAxisDeadzone ? 0 : (axis > 0 ? 1 : -1);
            if (isHorizontal)
            {
                if (_latestNaturalXAxis == currentValue)
                    return;
                _latestNaturalXAxis = currentValue;
            }
            else
            {
                if (_latestNaturalYAxis == currentValue)
                    return;
                _latestNaturalYAxis = currentValue;
            }

            if (currentValue != 0)
            {
                if (isHorizontal)
                    onHorizontalMvtDiscrete?.Invoke(currentValue);
                else
                {
                    onVerticalMvtDiscrete?.Invoke(currentValue);
                }
                if (showInputLog)
                    Debug.Log($"P{_playerInputIndex}, by {currentValue} (discrete)");
            }
        }
    }
}