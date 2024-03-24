using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Skin;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace MultiSuika.zOther
{
    public static class Initializer
    {
        #region Container

        public static List<ContainerInstance> InstantiateContainers(int playerCount,
            GameModeData gameModeData)
        {
            playerCount = playerCount <= 0 ? 1 : playerCount; // For cases like the lobby
            int containerToSpawn = UnityExtensions.DivideIntRoundedUp(playerCount, gameModeData.PlayerPerContainer);
            if (containerToSpawn <= 0)
                return null;

            List<ContainerInstance> instantiatedContainers = new List<ContainerInstance>();
            Vector2 distanceBetweenContainers = Vector2.zero;

            distanceBetweenContainers.x = (containerToSpawn > 1)
                ? Mathf.Abs(gameModeData.LeftmostContainerPositions[containerToSpawn - 1].x) * 2f /
                  (containerToSpawn - 1)
                : 0f;

            Transform objHolder = GetObjectsHolder();

            for (int i = 0; i < containerToSpawn; i++)
            {
                ContainerInstance newContainerInstance = Object.Instantiate(gameModeData.ContainerInstancePrefab);
                UnityExtensions.ResetLocalTransform(newContainerInstance.transform);

                instantiatedContainers.Add(newContainerInstance);

                GameObject containerParent = new GameObject($"Container ({(i + 1)})");
                containerParent.transform.SetParent(objHolder, false);
                newContainerInstance.ContainerParent = containerParent;

                containerParent.transform.position =
                    gameModeData.LeftmostContainerPositions[containerToSpawn - 1] +
                    (i * distanceBetweenContainers);
                containerParent.transform.localScale =
                    Vector3.one * gameModeData.ContainerScaling[containerToSpawn - 1];
            }

            return instantiatedContainers;
        }

        public static void SetContainersParameters(List<ContainerInstance> containers, GameModeData gameModeData)
        {
            for (int i = 0; i < containers.Count; ++i)
                SetContainerParameters(containers[i], gameModeData.SkinData.playersSkinData[i]);
        }

        private static void SetContainerParameters(ContainerInstance containerInstance, PlayerSkinData playerSkinData)
        {
            containerInstance.backgroundSpriteRenderer.sprite = playerSkinData.containerBackground;
            containerInstance.sideSpriteRenderer.sprite = playerSkinData.containerSide;
            containerInstance.failureSpriteRenderer.sprite = playerSkinData.containerFailure;
            containerInstance.successSpriteRenderer.sprite = playerSkinData.containerSuccess;
        }

        #endregion

        #region Cannon

        // public static List<CannonInstance> InstantiateCannons(int playerCount, GameModeData gameModeData,
        //     List<ContainerInstance> containers)
        // {
        //     if (!containers.Any() || playerCount <= 0)
        //         return null;
        //
        //     List<CannonInstance> instantiatedCannons = new List<CannonInstance>();
        //
        //     for (int i = 0; i < playerCount; i++)
        //     {
        //         ContainerInstance cannonContainerInstance =
        //             containers[GetContainerIndexForPlayer(i, gameModeData.playerPerContainer)];
        //         instantiatedCannons.Add(InstantiateCannon(gameModeData, cannonContainerInstance));
        //     }
        //
        //     return instantiatedCannons;
        // }

        // public static CannonInstance InstantiateCannon(GameModeData gameModeData, ContainerInstance containerInstance)
        // {
        //     var newCannon = Object.Instantiate(gameModeData.cannonInstancePrefab,
        //         containerInstance.ContainerParent.transform);
        //     UnityExtensions.ResetLocalTransform(newCannon.transform);
        //
        //     float xPos = gameModeData.isCannonSpawnXPosRandom
        //         ? Random.Range(-containerInstance.GetContainerHorizontalHalfLength(),
        //             containerInstance.GetContainerHorizontalHalfLength())
        //         : 0f;
        //     newCannon.transform.localPosition = new Vector2(xPos, gameModeData.cannonVerticalDistanceFromCenter);
        //
        //     return newCannon;
        // }

        // public static void SetCannonsParameters(List<CannonInstance> cannons, List<ContainerInstance> containers,
        //     GameModeData gameModeData)
        // {
        //     for (int i = 0; i < cannons.Count; ++i)
        //     {
        //         SetCannonParameters(i, cannons[i],
        //             containers[GetContainerIndexForPlayer(i, gameModeData.playerPerContainer)],
        //             gameModeData, gameModeData.skinData.playersSkinData[i]);
        //     }
        // }

        public static void SetCannonParameters(int playerIndex, CannonInstance cannonInstance, ContainerInstance containerInstance,
            GameModeData gameModeData, PlayerSkinData playerSkinData)
        {
            cannonInstance.SetPlayerIndex(playerIndex);
            
            cannonInstance.speed = gameModeData.CannonSpeed;
            cannonInstance.reloadCooldown = gameModeData.CannonReloadCooldown;
            cannonInstance.shootingForce = gameModeData.CannonShootingForce;
            cannonInstance.emptyDistanceBetweenBallAndCannon = gameModeData.DistanceBetweenBallAndCannon;
            cannonInstance.isUsingPeggleMode = gameModeData.IsCannonUsingPeggleMode;
            cannonInstance.horizontalMargin = containerInstance.GetContainerHorizontalHalfLength();

            cannonInstance.ballSetData = gameModeData.BallSetData;
            cannonInstance.ballSpriteData = playerSkinData.ballTheme;
            // cannonInstance.scoreReference = playerScoreRef;
            cannonInstance.containerInstance = containerInstance;
            cannonInstance.spriteRenderer.sprite = playerSkinData.cannonSprite;
        }


        #endregion
        
        private static Transform GetObjectsHolder()
        {
            var objects = GameObject.Find("Objects");
            if (objects == null)
                objects = new GameObject($"Objects");
            return objects.transform;
        }

        public static int GetContainerIndexForPlayer(int playerIndex, int playerPerContainer) =>
            UnityExtensions.DivideIntRoundedUp(playerIndex + 1, playerPerContainer) - 1;
    }
}