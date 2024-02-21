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
        [SerializeField] private float _containerMaxArea;
        [SerializeField] private float _accelerationValue;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _speedBarMax;
        [SerializeField] private float _comboDefaultMargin;
        
        [SerializeField] private TMP_Text _tmpScore;
        [SerializeField] private TMP_Text _tmpCombo;
        [SerializeField] private TMP_Text _tmpAreaFilled;
        [SerializeField] private TMP_Text _tmpSpeed;

        [SerializeField] private UIBlock2D _speedBar;
        
        public FloatReference ballAreaRef;
        private IntReference _playerScore;

        private float _percentageFilled = 0f;
        private int _ballFusedCombo = 1;
        private float _currentSpeed = 0f;
        // private float _speedTarget = 0f;

        
        
        
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

        public async void NewBallFused()
        {
            _ballFusedCombo += 1;
            int newComboValue = _ballFusedCombo;
            
            await Task.Delay((int)(_comboDefaultMargin * 1000));
            
            if (newComboValue == _ballFusedCombo)
                _ballFusedCombo = 1;
        }
        
        
        private void UpdateData()
        {
            _percentageFilled = ballAreaRef.Value * 100 / _containerMaxArea;

            if (_currentSpeed < _playerScore)
                _currentSpeed += _accelerationValue * _ballFusedCombo * Time.deltaTime;
        }
        
        private void UpdateDebugVisual()
        {
            // Score value
            _tmpScore.text = string.Format($"{_playerScore.Value}");
            
            // Combo value
            _tmpCombo.text = string.Format($"{_ballFusedCombo}");

            // Area Filled value
            _tmpAreaFilled.text = string.Format($"{_percentageFilled}");
            
            // Speed value
            _tmpSpeed.text = string.Format($"{_currentSpeed}");
            
            // Speed bar visual
            _speedBar.Size.Y = (_currentSpeed / _maxSpeed) * _speedBarMax;
            _speedBar.Color = Color.HSVToRGB((0.5f + _ballFusedCombo * 0.15f) % 1f, 0.65f, 0.9f);
        }
    }
}
