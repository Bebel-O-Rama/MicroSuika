using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MultiSuika.Manager;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerEffects : MonoBehaviour
    {
        [Header("VFXs")] 
        [SerializeField] private ParticleSystem _speedLines;
        [SerializeField] private ParticleSystem _glowEffect;
        [SerializeField] private SpriteRenderer _winOutsideSprite;
        [SerializeField] private ParticleSystem _loseExplosion;
        
        private int _playerIndex;

        
        private void Start()
        {
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(GetComponentInParent<ContainerInstance>());

            VersusManager.Instance.OnLeadStart.Subscribe(OnLeadStart, _playerIndex);
            VersusManager.Instance.OnLeadStop.Subscribe(OnLeadStop, _playerIndex);
            VersusManager.Instance.OnGameOver.Subscribe(OnGameOver, _playerIndex);
        }
        
        private void OnGameOver(bool hasWon)
        {
            if (hasWon)
            {
                if (_winOutsideSprite)
                    _winOutsideSprite.DOFade(1, 1);
            }
            else
            {
                if (_loseExplosion)
                    _loseExplosion.Play();
            }
        }
        
        private void OnLeadStart(float timerDuration)
        {
            _speedLines.Play();
        }
        
        private void OnLeadStop(bool x)
        {
            _speedLines.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

    }
}
