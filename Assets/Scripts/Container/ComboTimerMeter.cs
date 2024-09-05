using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MultiSuika.Manager;
using Nova;
using TMPro;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ComboTimerMeter : MonoBehaviour
    {
        [Header("Combo Text")]
        [SerializeField] private Color _noComboColor;
        [SerializeField] private Color _comboActiveColor;
        [SerializeField] private TMP_Text _comboTMP;
        
        [Header("Combo Timer")]
        [SerializeField] private UIBlock2D _comboTimerParentUIBlock2D;

        private int _playerIndex;
        private Sequence _timerSequence;
        private bool _isGameOver = false;
        
        private void Awake()
        {
            _timerSequence = DOTween.Sequence();        
        }
        
        void Start()
        {
            // NOTE: It's a workaround so that Nova's stuff can be parsed through the cameras
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            
            var containerInstance = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(containerInstance);
            
            VersusManager.Instance.OnGameOver.Subscribe(OnGameOver, _playerIndex);
            
            OnComboStop();

            ScoreManager.Instance.OnComboIncrement.Subscribe(OnComboIncrement, _playerIndex);
            ScoreManager.Instance.OnComboLost.Subscribe(OnComboStop, _playerIndex);
        }

        private void OnComboIncrement((int combo, float timer) args)
        {
            // Update text
            UpdateComboTextValue(args.combo + 1);

            if (_timerSequence.IsPlaying())
            {
                _timerSequence.Kill();
                // return;
            }
            
            _timerSequence = DOTween.Sequence();
                
            _timerSequence.Append(
                DOTween.To(() => _comboTimerParentUIBlock2D.Size.X.Percent, x => _comboTimerParentUIBlock2D.Size.X.Percent = x, 0f, 0.15f)
                    .SetEase(Ease.OutBack, 0.8f));

            _timerSequence.Append(
                DOTween.To(() => _comboTimerParentUIBlock2D.Size.X.Percent, x => _comboTimerParentUIBlock2D.Size.X.Percent = x, 1f, args.timer - 0.16f)
                    .SetEase(Ease.InSine, 0.5f));
        }

        private void OnComboStop(int comboLost = 1)
        {
            if (_timerSequence.IsActive())
            {
                _timerSequence.Kill();
            }

            if (_isGameOver)
                return;
            
            // Update text
            UpdateComboTextValue(1);

            // Set the timer back to zero
            _comboTimerParentUIBlock2D.Size.X.Percent = 1;
        }

        private void UpdateComboTextValue(int value)
        {
            _comboTMP.text = $"<size=70%>x</size>{value}";
            _comboTMP.color = value > 1 ? _comboActiveColor : _noComboColor;
        }

        private void OnGameOver(bool x)
        {
            if (_timerSequence.IsActive())
            {
                _timerSequence.Kill();
            }

            _isGameOver = true;
        }
    }
}
