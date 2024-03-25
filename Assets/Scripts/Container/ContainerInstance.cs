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
        [Header("Control Parameters")] 
        [SerializeField] [Min(0f)] private float _horizontalMvtHalfLength;

        [Header("Sprite Parameters")] 
        [SerializeField] private SpriteRenderer _backgroundSpriteRenderer;
        [SerializeField] private SpriteRenderer _sideSpriteRenderer;
        [SerializeField] private SpriteRenderer _failureSpriteRenderer;
        [SerializeField] private SpriteRenderer _successSpriteRenderer;
        
        public float HorizontalMvtHalfLength { get => _horizontalMvtHalfLength; }
        public Transform ContainerParent { get; private set; }

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
            _backgroundSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerBackground;
            _sideSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerSide;
            _failureSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerFailure;
            _successSpriteRenderer.sprite = gameModeData.SkinData.playersSkinData[containerIndex].containerSuccess;
        }
    }
}
