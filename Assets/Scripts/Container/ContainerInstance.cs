using System;
using DG.Tweening;
using MultiSuika.GameLogic;
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
        // [SerializeField] private SpriteRenderer _failureSpriteRenderer;
        // [SerializeField] private SpriteRenderer _successSpriteRenderer;

        [Header("VFXs")] 
        [SerializeField] private ParticleSystem _speedLines;
        [SerializeField] private ParticleSystem _glowEffect;
        [SerializeField] private SpriteRenderer _winOutsideSprite;
        [SerializeField] private ParticleSystem _loseExplosion;
        
        public float HorizontalMvtHalfLength { get => _horizontalMvtHalfLength; }
        public Transform ContainerParent { get; private set; }
        

        public void OnGameOver(bool hasWon)
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
            
            if (_winOutsideSprite)
                _winOutsideSprite.sprite = gameModeData.SkinData.GetPlayerSkinData(containerIndex).ContainerOutside;
        }
    }
}
