using System;
using MultiSuika.Container;
using MultiSuika.Utilities;
using Nova;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.DebugInfo
{
    public class ContainerRacingDebugInfo : MonoBehaviour
    {
        [Header("Speed Parameters")]
        [SerializeField] private float _speedBarMax; // 11

        [Header("Combo Parameters")]
        [SerializeField] private float _comboBarMax; // 5
        
        [Header("Visual Debug Parameters")] 
        [SerializeField] private Color _fixedDebugColor; // D2FFFD
        [SerializeField] private Color _risingDebugColor; // B2FFB7
        [SerializeField] private Color _reducingDebugColor; // FFB2B2
        
        [SerializeField] private TMP_Text _tmpScore;
        [SerializeField] private TMP_Text _tmpCombo;
        [SerializeField] private TMP_Text _tmpSpeed;
        [SerializeField] private TMP_Text _tmpTargetSpeed;
        [SerializeField] private TMP_Text _tmpAverageSpeed;
        [SerializeField] private TMP_Text _tmpAcceleration;
        [SerializeField] private TMP_Text _tmpRanking;
        [SerializeField] private TMP_Text _tmpRankingValue;
        [SerializeField] private TMP_Text _tmpSpeedBar;
        [SerializeField] private TMP_Text _tmpLeadTimer;

        [SerializeField] private UIBlock2D _speedBar;
        [SerializeField] private UIBlock2D _averageSpeedBar;
        [SerializeField] private UIBlock2D _speedTextBlock;
        [SerializeField] private UIBlock2D _comboBar;
        
        // Score parameters
        private IntReference _playerScore;
        
        // Area parameters
        private FloatReference _areaPercentFilled;
        
        // Speed parameters
        private FloatReference _currentSpeed;
        private FloatReference _averageSpeed;
        private FloatReference _targetSpeed;
        private FloatReference _maxSpeed;
        private float _previousSpeed;
        
        // Combo parameters
        private IntReference _combo;
        private FloatReference _comboTimer;
        private FloatReference _comboTimerFull;
        private FloatReference _acceleration;
        
        // Lead parameters
        private BoolReference _leadStatus;
        private FloatReference _leadTimer;
        
        // Ranking parameters
        private IntReference _ranking;
        
        // Position parameters
        private FloatReference _verticalPositionRatio;

        private GameObject _debugHolder;
        private bool _isDebugEnabled = false;
        
        private void Awake()
        {
            _debugHolder = transform.GetChild(0).gameObject;
            if (_debugHolder == null)
                gameObject.SetActive(false);
            _speedTextBlock.Color = _fixedDebugColor;
            _tmpLeadTimer.text = "";
        }

        private void Update()
        {
            if (!_isDebugEnabled)
                return;
            
            UpdateScoreParameters();
            UpdateComboParameters();
            UpdateSpeedParameters();
            UpdateLeadParameters();
            UpdateRankingParameters();
        }

        #region UpdateDebugInfo
        private void UpdateScoreParameters()
        {
            _tmpScore.text = string.Format($"{_playerScore.Value:0}");
        }
        
        private void UpdateComboParameters()
        {
            // Combo
            _tmpCombo.text = string.Format($"{_combo.Value}");
            _comboBar.Size.Y = (_comboTimer / _comboTimerFull) * _comboBarMax;
            _comboBar.Color = Color.HSVToRGB((0.5f + _combo * 0.15f) % 1f, 0.65f, 0.9f);
            
            // Acceleration
            _tmpAcceleration.text = string.Format($"{_acceleration * _combo:0.00}");
        }
        
        private void UpdateSpeedParameters()
        {
            // Speed text
            _tmpSpeed.text = string.Format($"{_currentSpeed.Value:0.00}");
            
            // Speed bar and text
            var speedBarSizePercent = _speedBar.Size.Percent;
            speedBarSizePercent.y = _currentSpeed / _maxSpeed;
            _speedBar.Size.Percent = speedBarSizePercent;
            _speedBar.Color = Color.HSVToRGB((0.5f + _combo * 0.15f) % 1f, 0.65f, 0.9f);
            
            _tmpSpeedBar.text = string.Format($"{_currentSpeed.Value:0.00}");
            if (Mathf.Abs(_previousSpeed - _currentSpeed) < Mathf.Epsilon)
            {
                _speedTextBlock.Color = _fixedDebugColor;
            }
            else
            {
                _speedTextBlock.Color = _currentSpeed > _previousSpeed ? _risingDebugColor : _reducingDebugColor;
            }
            _previousSpeed = _currentSpeed.Value;
            
            // Average Speed
            _tmpAverageSpeed.text = string.Format($"{_averageSpeed.Value:0.00}");
            var averageSpeedPositionPercent = _averageSpeedBar.Position.Percent;
            averageSpeedPositionPercent.y = _averageSpeed / _maxSpeed;
            _averageSpeedBar.Position.Percent = averageSpeedPositionPercent;
            
            // Target speed value
            _tmpTargetSpeed.text = string.Format($"{_targetSpeed.Value:0.00}");
        }

        private void UpdateLeadParameters()
        {
            if (_leadStatus)
                _tmpLeadTimer.text = _leadTimer > Mathf.Epsilon ? string.Format($"{_leadTimer.Value:0.0}") : "";
        }
        
        private void UpdateRankingParameters()
        {
            _tmpRanking.text = string.Format($"{_ranking.Value}");
            _tmpRankingValue.text = string.Format($"{(_verticalPositionRatio.Value * 100):00}%");
        }
        #endregion

        #region Setter
        public void SetScoreParameters(IntReference playerScore) => _playerScore = playerScore;

        public void SetBallAreaParameters(FloatReference areaPercentFilled) => _areaPercentFilled = areaPercentFilled;

        public void SetSpeedParameters(FloatReference currentSpeed, FloatReference averageSpeed, FloatReference targetSpeed, FloatReference maxSpeed)
        {
            _currentSpeed = currentSpeed;
            _averageSpeed = averageSpeed;
            _targetSpeed = targetSpeed;
            _maxSpeed = maxSpeed;
        }

        public void SetComboParameters(IntReference combo, FloatReference comboTimer, FloatReference comboTimerFull, FloatReference acceleration)
        {
            _combo = combo;
            _comboTimer = comboTimer;
            _comboTimerFull = comboTimerFull;
            _acceleration = acceleration;
        }
        
        public void SetLeadParameters(BoolReference leadStatus, FloatReference leadTimer)
        {
            _leadStatus = leadStatus;
            _leadTimer = leadTimer;
        }

        public void SetRankingParameters(IntReference ranking)
        {
            _ranking = ranking;
        }
        
        public void SetPositionParameters(FloatReference verticalPositionRatio)
        {
            _verticalPositionRatio = verticalPositionRatio;
        }

        public void SetDebugActive(bool isDebugActive)
        {
            _isDebugEnabled = isDebugActive;
            _debugHolder.SetActive(isDebugActive);
        }
        #endregion
    }
}
