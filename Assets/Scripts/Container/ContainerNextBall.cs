using System;
using System.Collections;
using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.GameLogic;
using MultiSuika.Skin;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerNextBall : MonoBehaviour
    {
        [SerializeField] private Transform _nextBallPosition;
        
        private int _playerIndex;
        private BallSetData _ballSetData;
        private BallSkinData _ballSkinData;
        private Transform _containerParentTransform;
        
        private BallInstance _nextBall;

        private void Start()
        {
            SpawnNextBall();
        }

        public BallInstance GetNextBall()
        {
            if (!_nextBall)
                SpawnNextBall();
            var ballToPass = _nextBall;
            SpawnNextBall();
            return ballToPass;
        }

        private void SpawnNextBall()
        {
            var ballIndex = _ballSetData.GetRandomBallTier();
            
            _nextBall = Instantiate(_ballSetData.BallInstancePrefab, _containerParentTransform);
            BallTracker.Instance.AddNewItem(_nextBall, _playerIndex);
            _nextBall.SetBallPosition(_nextBallPosition.localPosition, 10f);
            _nextBall.SetBallParameters(_playerIndex, ballIndex, _ballSetData, _ballSkinData);
            _nextBall.SetSimulatedParameters(false);

            _nextBall.SetBallScale(1);
        }


        #region Getter/Setter

        public SpriteRenderer GetNextBallSpriteRenderer() => _nextBall.transform.GetComponentInChildren<SpriteRenderer>();

        public void SetNextBallParameters(int playerIndex, GameModeData gameModeData)
        {
            _playerIndex = playerIndex;
            _ballSetData = gameModeData.BallSetData;
            _ballSkinData = gameModeData.SkinData.GetPlayerSkinData(_playerIndex).BallTheme;
            _containerParentTransform = ContainerTracker.Instance.GetParentTransformFromPlayer(_playerIndex);
        }

        #endregion
    }
}
