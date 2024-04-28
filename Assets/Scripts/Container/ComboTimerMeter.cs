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
        [SerializeField] private TMP_Text _comboTMP;
        
        [Header("Combo Timer")]
        [SerializeField] private UIBlock2D _comboTimerParentUIBlock2D;
        [SerializeField] private UIBlock2D _comboTimerColorHolderUIBlock2D;
        [SerializeField] private Color _fullColor;
        [SerializeField] private Color _emptyColor;
        
        private int _playerIndex;
        private Sequence _timerSequence;

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
            
            OnComboStop(1);

            ScoreManager.Instance.OnComboIncrement.Subscribe(OnComboIncrement, _playerIndex);
            ScoreManager.Instance.OnComboLost.Subscribe(OnComboStop, _playerIndex);
        }

        private void OnComboIncrement((int combo, float timer) args)
        {
            // Update text
            _comboTMP.text = string.Format($"x{args.combo + 1}");
            _comboTMP.color = _fullColor;

            if (_timerSequence.IsPlaying())
            {
                _timerSequence.Restart();
                return;
            }
            
            _timerSequence = DOTween.Sequence();
                
            _timerSequence.Append(
                DOTween.To(() => _comboTimerParentUIBlock2D.Size.X.Percent, x => _comboTimerParentUIBlock2D.Size.X.Percent = x, 1f, 0.15f)
                    .SetEase(Ease.OutBack, 0.8f));
            _timerSequence.Join(
                DOTween.To(() => _comboTimerColorHolderUIBlock2D.Color, x => _comboTimerColorHolderUIBlock2D.Color = x, _fullColor, 0.15f)
                    .SetEase(Ease.Linear));

            _timerSequence.Append(
                DOTween.To(() => _comboTimerParentUIBlock2D.Size.X.Percent, x => _comboTimerParentUIBlock2D.Size.X.Percent = x, 0f, args.timer - 0.16f)
                    .SetEase(Ease.InSine, 0.5f));
            _timerSequence.Join(
                DOTween.To(() => _comboTimerColorHolderUIBlock2D.Color, x => _comboTimerColorHolderUIBlock2D.Color = x, _emptyColor, args.timer - 0.16f)
                    .SetEase(Ease.InCubic, 2f));
        }

        private void OnComboStop(int comboLost)
        {
            if (_timerSequence.IsActive())
            {
                _timerSequence.Kill();
            }
            
            // Update text
            _comboTMP.text = "x1";
            _comboTMP.color = _noComboColor;

            // Manually update combo one last time
            _comboTimerParentUIBlock2D.Size.X.Percent = 0;
            _comboTimerColorHolderUIBlock2D.Color = _fullColor;
        }
    }
}
