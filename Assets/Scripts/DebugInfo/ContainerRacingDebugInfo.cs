using MultiSuika.Utilities;
using Nova;
using TMPro;
using UnityEngine;

namespace MultiSuika.DebugInfo
{
    public class ContainerRacingDebugInfo : MonoBehaviour
    {
        [Header("Speed parameters")]
        [SerializeField] private float _speedBarMaxHeight; // 11

        [Header("Combo parameters")]
        [SerializeField] private float _comboBarMaxHeight; // 5
        
        [Header("Color Debug parameters")] 
        [SerializeField] private Color _fixedDebugColor; // D2FFFD
        [SerializeField] private Color _risingDebugColor; // B2FFB7
        [SerializeField] private Color _reducingDebugColor; // FFB2B2
        
        [Header("Lead timer parameters")]
        [SerializeField] private TMP_Text _tmpLeadTimer;
        
        [Header("Progress bars parameters")]
        [SerializeField] private GameObject _progressBarsDebugHolder;
        [SerializeField] private UIBlock2D _speedBar;
        [SerializeField] private UIBlock2D _averageSpeedBar;
        [SerializeField] private UIBlock2D _speedTextBlock;
        [SerializeField] private UIBlock2D _comboBar;
        [SerializeField] private TMP_Text _tmpSpeedBar;

        [Header("Full debug text parameters")]
        [SerializeField] private GameObject _fullTextDebugHolder;
        [SerializeField] private TMP_Text _tmpScore;
        [SerializeField] private TMP_Text _tmpCombo;
        [SerializeField] private TMP_Text _tmpSpeed;
        [SerializeField] private TMP_Text _tmpTargetSpeed;
        [SerializeField] private TMP_Text _tmpAverageSpeed;
        [SerializeField] private TMP_Text _tmpAcceleration;
        [SerializeField] private TMP_Text _tmpRanking;
        [SerializeField] private TMP_Text _tmpPositionRatio;

        [Header("Abridged debug text parameters")]
        [SerializeField] private GameObject _abridgedTextDebugHolder; 
        [SerializeField] private TMP_Text _tmpTargetSpeedAbridged;
        [SerializeField] private TMP_Text _tmpComboAbridged;
        [SerializeField] private TMP_Text _tmpPositionRatioAbridged;
        
        // Score parameters
        private FloatReference _playerScore;
        
        // Area parameters
        private FloatReference _areaPercentFilled;
        
        // Speed parameters
        private FloatReference _currentSpeed;
        private FloatReference _averageSpeed;
        private FloatReference _targetSpeed;
        private FloatReference _speedSoftCap;
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
        
        // Debug activation parameters
        private BoolReference _isContainerSpeedBarDebugEnabled;
        private BoolReference _isContainerFullDebugTextEnabled;
        private BoolReference _isContainerAbridgedDebugTextEnabled;

        
        private void Awake()
        {
            _speedTextBlock.Color = _fixedDebugColor;
            _tmpLeadTimer.text = "";
        }

        private void Update()
        {
            _progressBarsDebugHolder.SetActive(_isContainerSpeedBarDebugEnabled);
            _abridgedTextDebugHolder.SetActive(_isContainerAbridgedDebugTextEnabled);
            _fullTextDebugHolder.SetActive(_isContainerFullDebugTextEnabled);
            
            if (_isContainerSpeedBarDebugEnabled)
                UpdateSpeedBarDebugInfo();
            if (_isContainerAbridgedDebugTextEnabled)
                UpdateAbridgedDebugInfoText();
            else if (_isContainerFullDebugTextEnabled)
                UpdateFullDebugInfoText();
            
            UpdateLeadDebugText();
        }

        #region UpdateDebugInfo

        private void UpdateSpeedBarDebugInfo()
        {
            // Speed text
            _tmpSpeedBar.text = string.Format($"{_currentSpeed.Value:0.00}");
            
            // Speed bar
            var speedBarSizePercent = _speedBar.Size.Percent;
            speedBarSizePercent.y = _currentSpeed / _speedSoftCap;
            _speedBar.Size.Percent = speedBarSizePercent;
            _speedBar.Color = Color.HSVToRGB((0.5f + _combo * 0.15f) % 1f, 0.65f, 0.9f);
            
            if (Mathf.Abs(_previousSpeed - _currentSpeed) < Mathf.Epsilon)
            {
                _speedTextBlock.Color = _fixedDebugColor;
            }
            else
            {
                _speedTextBlock.Color = _currentSpeed > _previousSpeed ? _risingDebugColor : _reducingDebugColor;
            }
            _previousSpeed = _currentSpeed.Value;
            
            // Average speed bar
            var averageSpeedPositionPercent = _averageSpeedBar.Position.Percent;
            averageSpeedPositionPercent.y = _averageSpeed / _speedSoftCap;
            _averageSpeedBar.Position.Percent = averageSpeedPositionPercent;
            
            // Combo bar
            _comboBar.Size.Y = (_comboTimer / _comboTimerFull) * _comboBarMaxHeight;
            _comboBar.Color = Color.HSVToRGB((0.5f + _combo * 0.15f) % 1f, 0.65f, 0.9f);
        }
        
        private void UpdateFullDebugInfoText()
        {
            // Score
            _tmpScore.text = string.Format($"{_playerScore.Value:0}");

            // Combo
            _tmpCombo.text = string.Format($"{_combo.Value}");
            
            // Acceleration
            _tmpAcceleration.text = string.Format($"{_acceleration * _combo:0.00}");
            
            // Current speed
            _tmpSpeed.text = string.Format($"{_currentSpeed.Value:0.00}");

            // Target speed
            _tmpTargetSpeed.text = string.Format($"{_targetSpeed.Value:0.00}");

            // Average speed
            _tmpAverageSpeed.text = string.Format($"{_averageSpeed.Value:0.00}");

            // Ranking
            _tmpRanking.text = string.Format($"{_ranking.Value}");
            
            // Position
            _tmpPositionRatio.text = string.Format($"{(_verticalPositionRatio.Value * 100):00}%");
        }
        
        private void UpdateAbridgedDebugInfoText()
        {
            // Target speed
            _tmpTargetSpeedAbridged.text = string.Format($"{_targetSpeed.Value:0.00}");
            
            // Combo
            _tmpComboAbridged.text = string.Format($"{_combo.Value}");
            
            // Position 
            _tmpPositionRatioAbridged.text = string.Format($"{(_verticalPositionRatio.Value * 100):00}%");
        }
        
        private void UpdateLeadDebugText()
        {
            if (_leadStatus)
                _tmpLeadTimer.text = _leadTimer > Mathf.Epsilon ? string.Format($"{_leadTimer.Value:0.0}") : "";
        }
        
        #endregion

        #region Setter
        public void SetScoreParameters(FloatReference playerScore) => _playerScore = playerScore;

        public void SetBallAreaParameters(FloatReference areaPercentFilled) => _areaPercentFilled = areaPercentFilled;

        public void SetSpeedParameters(FloatReference currentSpeed, FloatReference averageSpeed, FloatReference targetSpeed, FloatReference speedSoftCap)
        {
            _currentSpeed = currentSpeed;
            _averageSpeed = averageSpeed;
            _targetSpeed = targetSpeed;
            _speedSoftCap = speedSoftCap;
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

        public void SetDebugActivationParameters(BoolReference isContainerSpeedBarDebugEnabled,
            BoolReference isContainerFullDebugTextEnabled, BoolReference isContainerAbridgedDebugTextEnabled)
        {
            _isContainerSpeedBarDebugEnabled = isContainerSpeedBarDebugEnabled;
            _isContainerFullDebugTextEnabled = isContainerFullDebugTextEnabled;
            _isContainerAbridgedDebugTextEnabled = isContainerAbridgedDebugTextEnabled;
        }
        #endregion
    }
}
