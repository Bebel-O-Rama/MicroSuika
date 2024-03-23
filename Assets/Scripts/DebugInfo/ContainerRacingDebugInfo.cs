using System;
using System.Linq;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.ScoreSystemTransition;
using MultiSuika.Utilities;
using Nova;
using TMPro;
using UnityEngine;

namespace MultiSuika.DebugInfo
{
    public class ContainerRacingDebugInfo : MonoBehaviour
    {
        [Header("Activation parameters")]
        [SerializeField] private bool _isContainerSpeedBarDebugEnabled;
        [SerializeField] private bool _isContainerFullDebugTextEnabled;
        [SerializeField] private bool _isContainerAbridgedDebugTextEnabled;
        
        [Header("ScoreHandlerData parameters")]
        [SerializeField] private ScoreHandlerData _scoreHandlerData;
        
        [Header("Main UI parameters")]
        [SerializeField] private float _speedBarMaxHeight; // 11
        [SerializeField] private float _comboBarMaxHeight; // 5
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

        // Speed parameters
        private FloatReference _currentSpeed;
        private FloatReference _averageSpeed;
        private FloatReference _targetSpeed;
        private FloatReference _normalizedSpeed;
        private float _previousSpeed;
        
        // Combo parameters
        private int _combo;
        private float _onComboIncrementTimestamp;
        private float _comboTimerFull;
        private bool _isComboActive = false;
        // private FloatReference _acceleration;
        
        // // Lead parameters
        // private BoolReference _leadStatus;
        // private FloatReference _leadTimer;
        
        // // Ranking parameters
        // private IntReference _ranking;
        
        // // Position parameters
        // private FloatReference _verticalPositionRatio;
        
        // Debug activation parameters



        private int _playerIndex;

        private bool _isInLead = false;
        private float _leadTimer;
        
        private void Awake()
        {
            _speedTextBlock.Color = _fixedDebugColor;
            _tmpLeadTimer.text = "";
        }

        private void Start()
        {
            var container = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayersByItem(container).FirstOrDefault();

            _currentSpeed = ScoreManager.Instance.GetCurrentSpeedReference(_playerIndex);
            _averageSpeed = ScoreManager.Instance.GetAverageSpeedReference();
            _targetSpeed = ScoreManager.Instance.GetTargetSpeedReference(_playerIndex);
            _normalizedSpeed = ScoreManager.Instance.GetNormalizedSpeedReference(_playerIndex);

            VersusManager.Instance.OnLeadStart.Subscribe(OnLeadStart, _playerIndex);
            VersusManager.Instance.OnLeadStop.Subscribe(OnLeadStop, _playerIndex);
            
            ScoreManager.Instance.OnComboIncrement.Subscribe(OnComboIncrement, _playerIndex);
            ScoreManager.Instance.OnComboLost.Subscribe(OnComboStop, _playerIndex);
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

        private void OnComboIncrement((int combo, float comboTimerFull) args)
        {
            _combo = args.combo;
            _onComboIncrementTimestamp = Time.time;
            _comboTimerFull = args.comboTimerFull;
            _isComboActive = true;
        }

        private void OnComboStop(int combo)
        {
            _combo = 1;
            _isComboActive = false;
        }

        #region UpdateDebugInfo

        private void UpdateSpeedBarDebugInfo()
        {
            // Speed text
            _tmpSpeedBar.text = string.Format($"{_currentSpeed.Value:0.00}");
            
            // Speed bar
            var speedBarSizePercent = _speedBar.Size.Percent;
            speedBarSizePercent.y = _currentSpeed / _scoreHandlerData.SpeedSoftCap;
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
            
            // // Average speed bar
            var averageSpeedPositionPercent = _averageSpeedBar.Position.Percent;
            averageSpeedPositionPercent.y = _averageSpeed / _scoreHandlerData.SpeedSoftCap;
            _averageSpeedBar.Position.Percent = averageSpeedPositionPercent;
            
            // Combo bar
            if (_isComboActive)
            {
                _comboBar.Size.Y = (_comboTimerFull - (Time.time - _onComboIncrementTimestamp)) / _comboTimerFull * _comboBarMaxHeight ;
                _comboBar.Color = Color.HSVToRGB((0.5f + _combo * 0.15f) % 1f, 0.65f, 0.9f);
            }
            else
            {
                _comboBar.Size.Y = 0f;
            }
        }
        
        private void UpdateFullDebugInfoText()
        {
            // Score
            _tmpScore.text = string.Format($"{_playerScore.Value:0}");

            // Combo
            _tmpCombo.text = string.Format($"{_combo}");
            
            // Acceleration
            _tmpAcceleration.text = string.Format($"{_scoreHandlerData.BaseAcceleration * _combo:0.00}");
            
            // Current speed
            _tmpSpeed.text = string.Format($"{_currentSpeed.Value:0.00}");

            // Target speed
            _tmpTargetSpeed.text = string.Format($"{_targetSpeed.Value:0.00}");

            // Average speed
            _tmpAverageSpeed.text = string.Format($"{_averageSpeed.Value:0.00}");

            // Ranking
            _tmpRanking.text = string.Format($"{ScoreManager.Instance.GetPlayerRanking(_playerIndex):0}");
            
            // Position
            _tmpPositionRatio.text = string.Format($"{(_normalizedSpeed.Value * 100):00}%");
        }
        
        private void UpdateAbridgedDebugInfoText()
        {
            // Target speed
            _tmpTargetSpeedAbridged.text = string.Format($"{_targetSpeed.Value:0.00}");
            
            // Combo
            _tmpComboAbridged.text = string.Format($"{_combo}");
            
            // Position 
            _tmpPositionRatioAbridged.text = string.Format($"{(_normalizedSpeed.Value * 100):00}%");
        }
        
        private void UpdateLeadDebugText()
        {
            if (!_isInLead) 
                return;
            _tmpLeadTimer.text = _leadTimer > Mathf.Epsilon ? string.Format($"{_leadTimer:0.0}") : "";
            _leadTimer -= Time.deltaTime;
        }

        private void OnLeadStart(float timer)
        {
            _leadTimer = timer;
            _isInLead = true;
        }

        private void OnLeadStop(bool x)
        {
            _isInLead = false;
        }
        
        #endregion

        #region Setter
        // public void SetScoreParameters(FloatReference playerScore) => _playerScore = playerScore;
        //
        // public void SetBallAreaParameters(FloatReference areaPercentFilled) => _areaPercentFilled = areaPercentFilled;
        //
        // public void SetSpeedParameters(FloatReference currentSpeed, FloatReference averageSpeed, FloatReference targetSpeed, FloatReference speedSoftCap)
        // {
        //     _currentSpeed = currentSpeed;
        //     _averageSpeed = averageSpeed;
        //     _targetSpeed = targetSpeed;
        //     _speedSoftCap = speedSoftCap;
        // }
        //
        // public void SetComboParameters(IntReference combo, FloatReference comboTimer, FloatReference comboTimerFull, FloatReference acceleration)
        // {
        //     _combo = combo;
        //     _comboTimer = comboTimer;
        //     _comboTimerFull = comboTimerFull;
        //     _acceleration = acceleration;
        // }
        //
        // public void SetLeadParameters(BoolReference leadStatus, FloatReference leadTimer)
        // {
        //     _leadStatus = leadStatus;
        //     _leadTimer = leadTimer;
        // }
        //
        // public void SetRankingParameters(IntReference ranking)
        // {
        //     _ranking = ranking;
        // }
        
        // public void SetPositionParameters(FloatReference verticalPositionRatio)
        // {
        //     _verticalPositionRatio = verticalPositionRatio;
        // }
        
        // public void SetDebugActivationParameters(BoolReference isContainerSpeedBarDebugEnabled,
        //     BoolReference isContainerFullDebugTextEnabled, BoolReference isContainerAbridgedDebugTextEnabled)
        // {
        //     _isContainerSpeedBarDebugEnabled = isContainerSpeedBarDebugEnabled;
        //     _isContainerFullDebugTextEnabled = isContainerFullDebugTextEnabled;
        //     _isContainerAbridgedDebugTextEnabled = isContainerAbridgedDebugTextEnabled;
        // }
        #endregion
    }
}
