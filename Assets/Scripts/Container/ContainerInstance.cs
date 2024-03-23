using System;
using System.Collections.Generic;
using MultiSuika.Ball;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerInstance : MonoBehaviour
    {
        [Header ("Control Parameters")]
        [SerializeField] [Min(0f)] public float horizontalMvtHalfLength;

        [Header("Sprite Parameters")]
        [SerializeField] public SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] public SpriteRenderer sideSpriteRenderer;
        [SerializeField] public SpriteRenderer failureSpriteRenderer;
        [SerializeField] public SpriteRenderer successSpriteRenderer;

        [Header("Audio Parameters")]
        [SerializeField] public List<ContainerCollisionCheck> collisionChecks;
        [SerializeField] public float ballAudioVelocityMin;

        public AK.Wwise.RTPC rtpc_ballImpactVelocity;
        
        public AK.Wwise.Event WwiseEventBallContainerCollisionT0;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT1;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT2;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT3;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT4;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT5;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT6;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT7;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT8;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT9;
        public AK.Wwise.Event WwiseEventBallContainerCollisionT10;
        
        
        private GameObject _containerParent;
    
        // TODO: What the heck is this???
        public GameObject ContainerParent
        {
            get => _containerParent;
            set
            {
                _containerParent = value;
                transform.SetParent(_containerParent.transform);
                foreach (var collCheck in collisionChecks)
                {
                    collCheck.SetContainerCollisionCheck(this, ballAudioVelocityMin);
                }
            }
        }
    
        public float GetContainerHorizontalHalfLength() => horizontalMvtHalfLength;

        public void OnGameOver(bool hasWon)
        {
            if (hasWon)
            {
                successSpriteRenderer.enabled = true;
            }
            else
            {
                failureSpriteRenderer.enabled = true;
            }
        }

        public void OnBallCollision(float velocity, BallInstance ballInstance)
        {
            rtpc_ballImpactVelocity.SetValue(gameObject, velocity);
            switch (ballInstance.BallTierIndex)
            {
                case 0:
                    WwiseEventBallContainerCollisionT0.Post(ballInstance.gameObject);
                    break;
                case 1:
                    WwiseEventBallContainerCollisionT1.Post(ballInstance.gameObject);
                    break;
                case 2:
                    WwiseEventBallContainerCollisionT2.Post(ballInstance.gameObject);
                    break;
                case 3:
                    WwiseEventBallContainerCollisionT3.Post(ballInstance.gameObject);
                    break;
                case 4:
                    WwiseEventBallContainerCollisionT4.Post(ballInstance.gameObject);
                    break;
                case 5:
                    WwiseEventBallContainerCollisionT5.Post(ballInstance.gameObject);
                    break;
                case 6:
                    WwiseEventBallContainerCollisionT6.Post(ballInstance.gameObject);
                    break;
                case 7:
                    WwiseEventBallContainerCollisionT7.Post(ballInstance.gameObject);
                    break;
                case 8:
                    WwiseEventBallContainerCollisionT8.Post(ballInstance.gameObject);
                    break;
                case 9:
                    WwiseEventBallContainerCollisionT9.Post(ballInstance.gameObject);
                    break;
                case 10:
                    WwiseEventBallContainerCollisionT10.Post(ballInstance.gameObject);
                    break;
            }
        }
    }
}
