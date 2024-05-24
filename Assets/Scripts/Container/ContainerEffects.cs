using System;
using System.Collections;
using DG.Tweening;
using MultiSuika.Cannon;
using MultiSuika.Manager;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerEffects : MonoBehaviour
    {
        [Header("VFXs")] 
        [SerializeField] private ParticleSystem _speedLines;
        [SerializeField] private SpriteRenderer _winOutsideSprite;
        [SerializeField] private ParticleSystem _loseExplosion;
        [SerializeField] private ParticleSystem _glowEffect;

        [Header("Lead Parameters")]
        [SerializeField] private float _speedLinesMinScale; // 3
        [SerializeField] private float _speedLinesMaxScale; // 5
        [SerializeField] private float _speedLinesMinRateOverEmission; // 100
        [SerializeField] private float _speedLinesMaxRateOverEmission; // 250
        
        [SerializeField] private float _glowDuration;

        [Header("Win Parameters")] 
        [SerializeField] private SpriteRenderer _containerSkin;
        
        private int _playerIndex;
        private Sequence _speedLinesSequence;

        private void Awake()
        {
            _speedLinesSequence = DOTween.Sequence();
        }

        private void Start()
        {
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(GetComponentInParent<ContainerInstance>());

            _winOutsideSprite.sprite = VersusManager.Instance.GetContainerOutsideSprite(_playerIndex);
            
            VersusManager.Instance.OnLeadStart.Subscribe(OnLeadStart, _playerIndex);
            VersusManager.Instance.OnLeadStop.Subscribe(OnLeadStop, _playerIndex);
            VersusManager.Instance.OnGameOver.Subscribe(OnGameOver, _playerIndex);
        }

        #region GameOver

        private void OnGameOver(bool hasWon)
        {
            if (_speedLinesSequence.IsPlaying())
            {
                _speedLinesSequence.Kill();
            }
            _speedLines.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _glowEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (hasWon)
            {
                OnWin();
            }
            else
            {
                OnLose();
            }
        }

        private void OnWin()
        {
            var nextBallSpriteRenderer = CannonTracker.Instance.GetItemFromPlayerOrDefault(_playerIndex)
                .GetNextBallSpriteRenderer();
            var cannonSpriteRenderer = CannonTracker.Instance.GetItemFromPlayerOrDefault(_playerIndex).spriteRenderer;
            
            var winSequence = DOTween.Sequence();
            winSequence.Append(_winOutsideSprite.DOFade(1, 1))
                .Join(_containerSkin.DOFade(0, 1).SetEase(Ease.InQuart))
                .Join(nextBallSpriteRenderer.DOFade(0, 1).SetEase(Ease.InQuart))
                .Join(cannonSpriteRenderer.DOFade(0, 1).SetEase(Ease.InQuart));
            
            
            _winOutsideSprite.DOFade(1, 1);
        }

        private void OnLose()
        {
            _loseExplosion.Play();
        }

        #endregion

        #region Lead

        private void OnLeadStart(float timerDuration)
        {
            if (_speedLinesSequence.IsPlaying())
            {
                _speedLinesSequence.Kill();
            }
            _speedLinesSequence = DOTween.Sequence();
            
            // For now, let's say that the minimum timer duration will safeguard us for any issue there
            var speedLinesRampUpDuration = Mathf.Max(timerDuration - _glowDuration, 1);

            var speedLineEmission = _speedLines.emission;
            
            _speedLines.transform.localScale = Vector3.one * _speedLinesMinScale;
            speedLineEmission.rateOverTime = _speedLinesMinRateOverEmission;
            
            _speedLinesSequence.AppendCallback(() => _speedLines.Play())
                .Append(_speedLines.transform.DOScale(_speedLinesMaxScale, speedLinesRampUpDuration))
                .Join(DOTween.To(() => speedLineEmission.rateOverTime.constant,
                    x => speedLineEmission.rateOverTime = x, _speedLinesMaxRateOverEmission,
                    speedLinesRampUpDuration))
                .AppendCallback(() => _glowEffect.Play());
        }

        private void OnLeadStop(bool x)
        {
            if (_speedLinesSequence.IsPlaying())
            {
                _speedLinesSequence.Kill();
            }
            _speedLines.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _glowEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        #endregion
    }
}