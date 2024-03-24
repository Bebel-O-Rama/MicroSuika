using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.Skin;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Container
{
    public class ContainerInstance : MonoBehaviour
    {
        [FormerlySerializedAs("horizontalMvtHalfLength")] [Header("Control Parameters")] [SerializeField] [Min(0f)]
        private float _horizontalMvtHalfLength;

        [FormerlySerializedAs("backgroundSpriteRenderer")] [Header("Sprite Parameters")] [SerializeField]
        private SpriteRenderer _backgroundSpriteRenderer;

        [FormerlySerializedAs("sideSpriteRenderer")] [SerializeField]
        private SpriteRenderer _sideSpriteRenderer;

        [FormerlySerializedAs("failureSpriteRenderer")] [SerializeField]
        private SpriteRenderer _failureSpriteRenderer;

        [FormerlySerializedAs("successSpriteRenderer")] [SerializeField]
        private SpriteRenderer _successSpriteRenderer;

        [Header("Audio Parameters")] // Clean this up
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
        
        public float HorizontalMvtHalfLength { get => _horizontalMvtHalfLength; }
        public Transform ContainerParent { get; private set; }


        private void Start()
        {
            // Switch to the Signal2DCollision
            foreach (var collCheck in collisionChecks)
            {
                collCheck.SetContainerCollisionCheck(this, ballAudioVelocityMin);
            }
        }

        public void OnGameOver(bool hasWon)
        {
            if (hasWon)
            {
                _successSpriteRenderer.enabled = true;
            }
            else
            {
                _failureSpriteRenderer.enabled = true;
            }
        }

        // Dear god, remove this mess!
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

        public void SetContainerParameters(GameModeData gameModeData, int containerIndex = 0, int containerToSpawn = 1)
        {
            transform.ResetLocalTransform();
            
            ContainerParent = transform.parent.transform;
            foreach (var collCheck in collisionChecks)
            {
                collCheck.SetContainerCollisionCheck(this, ballAudioVelocityMin);
            }
            
            ContainerParent.SetLayerRecursively(LayerMask.NameToLayer($"Container{containerIndex + 1}"));
            

            Vector2 distanceBetweenContainers = Vector2.zero;
            distanceBetweenContainers.x = (containerToSpawn > 1)
                ? Mathf.Abs(gameModeData.LeftmostContainerPositions[containerToSpawn - 1].x) * 2f /
                  (containerToSpawn - 1)
                : 0f;

            ContainerParent.position =
                gameModeData.LeftmostContainerPositions[containerToSpawn - 1] +
                (containerIndex * distanceBetweenContainers);
            ContainerParent.localScale =
                Vector3.one * gameModeData.ContainerScaling[containerToSpawn - 1];
            
            _backgroundSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerBackground;
            _sideSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerSide;
            _failureSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerFailure;
            _successSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerSuccess;
        }
    }
}
