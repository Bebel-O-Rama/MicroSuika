using System;
using System.Collections;
using System.Collections.Generic;
using MultiSuika.Container;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Skin
{
    public class SpeedMeterSkin : MonoBehaviour
    {
        [SerializeField] private List<Texture2D> _playerSpeedShaderMasks;

        private int _playerIndex;
        private ContainerInstance _containerInstance;
        private Material _containerSideMat;


        #region UsingCombo

        private float _currentTimerFull;
        private float _comboIncrementTimestamp;
        private int _currentCombo;
        
        private float _comboFilledLevel;

        private void Awake()
        {
            _containerSideMat = GetComponent<SpriteRenderer>().material;
        }

        void Start()
        {
            _containerInstance = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(_containerInstance);
            _containerSideMat.SetTexture("_ShaderMask", _playerSpeedShaderMasks[_playerIndex]);
            
            ScoreManager.Instance.OnComboIncrement.Subscribe(OnComboIncrement, _playerIndex);
            ScoreManager.Instance.OnComboLost.Subscribe(OnComboLost, _playerIndex);
        }

        void Update()
        {
            Debug.Log((_currentTimerFull - (Time.time - _comboIncrementTimestamp)) / _currentTimerFull);
            _comboFilledLevel = Mathf.Clamp01((_currentTimerFull - (Time.time - _comboIncrementTimestamp)) / _currentTimerFull);
            _containerSideMat.SetFloat("_FilledLevel", _comboFilledLevel);
        }

        private void OnComboIncrement((int comboLevel, float timer) args)
        {
            Debug.Log("BOUP!--------------------------------------------------------------------");
            Debug.Log($"args = {args.comboLevel} and {args.timer}");
            _currentCombo = args.comboLevel;
            _currentTimerFull = args.timer;
            _comboIncrementTimestamp = Time.time;
        }

        public void OnComboLost(int comboLevelLost)
        {
            
        }
        

        #endregion

        #region UsingSpeed

        // private FloatReference _currentSpeed;
        // private FloatReference _speedConditionCeiling;
        // private float _normalizedSpeedFilledLevel;
        //
        // private void Awake()
        // {
        //     _containerSideMat = GetComponent<SpriteRenderer>().material;
        // }
        //
        // void Start()
        // {
        //     _containerInstance = GetComponentInParent<ContainerInstance>();
        //     _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(_containerInstance);
        //     _containerSideMat.SetTexture("_ShaderMask", _playerSpeedShaderMasks[_playerIndex]);
        //     _currentSpeed = ScoreManager.Instance.GetCurrentSpeedReference(_playerIndex);
        //
        //     _speedConditionCeiling = VersusManager.Instance.GetSpeedConditionCeiling();
        // }
        //
        // void Update()
        // {
        //     _normalizedSpeedFilledLevel = Mathf.Clamp01(_currentSpeed / _speedConditionCeiling);
        //     _containerSideMat.SetFloat("_FilledLevel", _normalizedSpeedFilledLevel);
        // }

        #endregion
    }
}