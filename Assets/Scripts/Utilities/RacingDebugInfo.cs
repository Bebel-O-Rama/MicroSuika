using System;
using Nova;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Utilities
{
    public class RacingDebugInfo : MonoBehaviour
    {
        [Header("Damping Parameters")]
        [SerializeField] private DampingMethod _dampingMethod;
        [SerializeField] [Range (0f, 1f)] private float _fixedPercent;
        [SerializeField] [Min (0f)] private float _fixedValue;
        [SerializeField] private AnimationCurve _curvePercent;
        
        [Header("Container Parameters")]
        [SerializeField] private float _containerMaxArea;
        
        [Header("Speed Parameters")]
        [SerializeField] private float _accelerationValue;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _speedBarMax;
        
        [Header("Combo Parameters")]
        [SerializeField] private float _comboBarMax;
        [SerializeField] private float _comboTimerDuration;

        [Header("Visual Debug Parameters")] 
        [SerializeField] private Color _fixedDebugColor;
        [SerializeField] private Color _risingDebugColor;
        [SerializeField] private Color _reducingDebugColor;
        
        [SerializeField] private TMP_Text _tmpScore;
        [SerializeField] private TMP_Text _tmpAreaFilled;
        [SerializeField] private TMP_Text _tmpCombo;
        [SerializeField] private TMP_Text _tmpSpeed;
        [SerializeField] private TMP_Text _tmpTargetSpeed;
        [SerializeField] private TMP_Text _tmpAverageSpeed;
        [SerializeField] private TMP_Text _tmpAcceleration;
        [SerializeField] private TMP_Text _tmpDamping;
        [SerializeField] private TMP_Text _tmpDampingMethod;
        [SerializeField] private TMP_Text _tmpRanking;
        [SerializeField] private TMP_Text _tmpSpeedBar;
        
        
        [SerializeField] private UIBlock2D _speedBar;
        [SerializeField] private UIBlock2D _averageSpeedBar;
        [SerializeField] private UIBlock2D _speedTextBlock;
        [SerializeField] private UIBlock2D _comboBar;
        
        public FloatReference ballAreaRef;
        public FloatReference currentSpeed;
        public FloatReference averageSpeed;
        public IntReference currentRanking;

        private IntReference _playerScore;
        private float _percentageFilled = 0f;
        private int _combo = 1;
        private float _targetSpeed = 0f;
        private Vector2 _currentComboTimer;

        private Color _debugSpeedTextBlockColor;
        
        private enum DampingMethod
        {
            None,
            FixedPercent,
            Fixed,
            AnimCurve
        }
        
        private void Start()
        {
            _playerScore = transform.parent.GetComponentInChildren<Cannon.Cannon>().scoreReference;
            if (_containerMaxArea < 0)
                _containerMaxArea = 1;
        }

        private void Update()
        {
            UpdateData();
            UpdateDebugVisual();
        }

        public void NewBallFused(float scoreValue)
        {
            _combo += 1;
            _currentComboTimer = new Vector2(_comboTimerDuration, _comboTimerDuration);
            _targetSpeed += scoreValue * 2f;
        }
        
        
        private void UpdateData()
        {
            // Percentage filled value
            _percentageFilled = ballAreaRef.Value * 100f / _containerMaxArea;
            
            // Combo value
            if (_combo > 1)
            {
                _currentComboTimer.x -= Time.deltaTime;
                if (_currentComboTimer.x < Mathf.Epsilon)
                    _combo = 1;
            }
            
            // Damping value
            _targetSpeed -= _targetSpeed - GetDampingValue() > 0 ? GetDampingValue() : 0f;
            
            // Speed value
            var previousSpeed = currentSpeed.Value;
            currentSpeed.Variable.SetValue(Mathf.MoveTowards(currentSpeed, _targetSpeed, _accelerationValue * _combo * Time.deltaTime));

            if (Mathf.Abs(previousSpeed - currentSpeed) < Mathf.Epsilon)
            {
                _debugSpeedTextBlockColor = _fixedDebugColor;
            }
            else
            {
                _debugSpeedTextBlockColor = currentSpeed > previousSpeed ? _risingDebugColor : _reducingDebugColor;
            }
        }
        
        private void UpdateDebugVisual()
        {
            // Score value
            _tmpScore.text = string.Format($"{_playerScore.Value:0}");
            
            // Percentage filled value
            _tmpAreaFilled.text = string.Format($"{_percentageFilled:0.00}");
            
            // Combo value
            _tmpCombo.text = string.Format($"{_combo}");
            _comboBar.Size.Y = (_currentComboTimer.x / _currentComboTimer.y) * _comboBarMax;
            _comboBar.Color = Color.HSVToRGB((0.5f + _combo * 0.15f) % 1f, 0.65f, 0.9f);

            // Speed value
            _tmpSpeed.text = string.Format($"{currentSpeed.Value:0.00}");
            _tmpSpeedBar.text = string.Format($"{currentSpeed.Value:0.00}");
            _speedTextBlock.Color = _debugSpeedTextBlockColor;
            var speedBarSizePercent = _speedBar.Size.Percent;
            speedBarSizePercent.y = currentSpeed / _maxSpeed;
            _speedBar.Size.Percent = speedBarSizePercent;
            _speedBar.Color = Color.HSVToRGB((0.5f + _combo * 0.15f) % 1f, 0.65f, 0.9f);
            
            // Average Speed
            _tmpAverageSpeed.text = string.Format($"{averageSpeed.Value:0.00}");
            var averageSpeedPositionPercent = _averageSpeedBar.Position.Percent;
            averageSpeedPositionPercent.y = averageSpeed / _maxSpeed;
            _averageSpeedBar.Position.Percent = averageSpeedPositionPercent;
            
            
            // Target speed value
            _tmpTargetSpeed.text = string.Format($"{_targetSpeed:0.00}");
            
            // Speed diff value
            
            
            // Acceleration value
            _tmpAcceleration.text = string.Format($"{_accelerationValue * _combo:0.00}");
            
            // Damping value
            _tmpDamping.text = string.Format($"{GetDampingValue():0.00}");
            
            // Damping value
            _tmpDampingMethod.text = string.Format($"{_dampingMethod}");
            
            // Ranking value
            _tmpRanking.text = string.Format($"{currentRanking.Value}");
        }

        private float GetDampingValue()
        {
            return _dampingMethod switch
            {
                DampingMethod.None => 0f,
                DampingMethod.FixedPercent => currentSpeed * _fixedPercent * Time.deltaTime,
                DampingMethod.Fixed => _fixedValue * Time.deltaTime,
                DampingMethod.AnimCurve => currentSpeed * _curvePercent.Evaluate(currentSpeed / _maxSpeed) * Time.deltaTime,
                _ => 0f
            };
        }
    }
}
