using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MultiSuika.Manager;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ComboTimerMeter : MonoBehaviour
    {
        [SerializeField] private Transform _barTransform;
        [SerializeField] private SpriteRenderer _barSpriteRenderer;
        [SerializeField] private Color _fullColor;
        [SerializeField] private Color _emptyColor;
        
        private int _playerIndex;
        private ContainerInstance _containerInstance;
        
        private Vector3 _tfScale = Vector3.one;

        private bool _isComboActive = false;

        // private Tweener _timerTweener;
        private Sequence _timerSequence;

        private void Awake()
        {
            _timerSequence = DOTween.Sequence();        
        }
        
        void Start()
        {
            _containerInstance = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(_containerInstance);
            
            UpdateXScale(0);
            _barSpriteRenderer.color = _fullColor;

            ScoreManager.Instance.OnComboIncrement.Subscribe(OnComboIncrement, _playerIndex);
            ScoreManager.Instance.OnComboLost.Subscribe(OnComboStop, _playerIndex);


            // DOTween.defaultAutoPlay = AutoPlay.AutoPlayTweeners;

            // _timerSequenceReset.Append(_barSpriteRenderer.DOColor(_fullColor, 0.1f).SetEase(Ease.OutElastic));
            // _timerSequenceReset.Join(_barTransform.DOScaleX(0, 0.1f).SetEase(Ease.OutElastic));
            //
            // _timerSequenceReset.Append(_barSpriteRenderer.DOColor(_emptyColor, 0.1f).SetEase(Ease.OutElastic));
            // _timerSequenceReset.Join(_barTransform.DOScaleX(0, 0.1f).SetEase(Ease.OutElastic));

        }

        // void Update()
        // {
        //     if (!_isComboActive)
        //         return;
        //     
        //     
        //     
        // }

        private void OnComboIncrement((int combo, float timer) args)
        {
            // UpdateXScale(1);
            // _barSpriteRenderer.color = _fullColor;
            //
            // if (_timerSequence.IsActive())
            // {
            //     _timerSequence.Restart();
            // }
            _timerSequence = DOTween.Sequence();        

            _timerSequence.Append(_barTransform.DOScaleX(1, 0.15f).SetEase(Ease.OutBack, 0.8f));
            _timerSequence.Join(_barSpriteRenderer.DOColor(_fullColor, 0.15f).SetEase(Ease.Linear));
            
            _timerSequence.Append(_barTransform.DOScaleX(0, args.timer).SetEase(Ease.InSine, 0.5f));
            _timerSequence.Join(_barSpriteRenderer.DOColor(_emptyColor, args.timer).SetEase(Ease.InSine, 1.3f));
            
            // _timerSequence.Kill();
        }

        private void OnComboStop(int comboLost)
        {
            _isComboActive = false;
            UpdateXScale(0);
        }

        private void StartTimerTween(float timerFull)
        {

        }

        private void UpdateXScale(float horizontalScale)
        {
            _tfScale.x = horizontalScale;
            _barTransform.localScale = _tfScale;
        }
    }
}
