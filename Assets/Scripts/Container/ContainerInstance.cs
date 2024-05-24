using System;
using DG.Tweening;
using MultiSuika.GameLogic;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerInstance : MonoBehaviour
    {
        [Header("Control Parameters")] 
        [SerializeField] [Min(0f)] private float _horizontalMvtHalfLength;

        [Header("Sprite Parameters")] 
        [SerializeField] private SpriteRenderer _backgroundSpriteRenderer;
        [SerializeField] private SpriteRenderer _sideSpriteRenderer;
        [SerializeField] private SpriteRenderer _nextBallHolderSpriteRenderer;


        
        public float HorizontalMvtHalfLength { get => _horizontalMvtHalfLength; }
        public Transform ContainerParent { get; private set; }

        // private int _playerIndex;
        
        // private void Start()
        // {
        //     _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(this);
        //
        //     VersusManager.Instance.OnLeadStart.Subscribe(OnLeadStart, _playerIndex);
        //     VersusManager.Instance.OnLeadStop.Subscribe(OnLeadStop, _playerIndex);
        // }

        // public void OnGameOver(bool hasWon)
        // {
        //     if (hasWon)
        //     {
        //         if (_winOutsideSprite)
        //             _winOutsideSprite.DOFade(1, 1);
        //     }
        //     else
        //     {
        //         if (_loseExplosion)
        //             _loseExplosion.Play();
        //     }
        // }
        //
        // private void OnLeadStart(float timerDuration)
        // {
        //     _speedLines.Play();
        // }
        //
        // private void OnLeadStop(bool x)
        // {
        //     // stop lead
        // }

        public void SetContainerParameters(GameModeData gameModeData, int containerIndex = 0, int containerToSpawn = 1)
        {
            // Set layers
            transform.ResetLocalTransform();
            ContainerParent = transform.parent.transform;
            ContainerParent.SetLayerRecursively(LayerMask.NameToLayer($"Container{containerIndex + 1}"));
            
            // Set position
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
            
            // Set sprites
            _backgroundSpriteRenderer.sprite = gameModeData.SkinData.GetPlayerSkinData(containerIndex).ContainerBackground;
            _sideSpriteRenderer.sprite = gameModeData.SkinData.GetPlayerSkinData(containerIndex).ContainerSide;
            _nextBallHolderSpriteRenderer.sprite =
                gameModeData.SkinData.GetPlayerSkinData(containerIndex).ContainerNextBallHolder;
        }
    }
}
