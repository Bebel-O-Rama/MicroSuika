using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Player;
using MultiSuika.Skin;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace MultiSuika.Utilities
{
    public static class Initializer
    {
        #region Container

        public static List<ContainerInstance> InstantiateContainers(int playerCount,
            GameModeData gameModeData)
        {
            playerCount = playerCount <= 0 ? 1 : playerCount; // For cases like the lobby
            int containerToSpawn = DivideIntRoundedUp(playerCount, gameModeData.playerPerContainer);
            if (containerToSpawn <= 0)
                return null;

            List<ContainerInstance> instantiatedContainers = new List<ContainerInstance>();
            Vector2 distanceBetweenContainers = Vector2.zero;

            distanceBetweenContainers.x = (containerToSpawn > 1)
                ? Mathf.Abs(gameModeData.leftmostContainerPositions[containerToSpawn - 1].x) * 2f /
                  (containerToSpawn - 1)
                : 0f;

            Transform objHolder = GetObjectsHolder();

            for (int i = 0; i < containerToSpawn; i++)
            {
                ContainerInstance newContainerInstance = Object.Instantiate(gameModeData.containerInstancePrefab);
                ResetLocalTransform(newContainerInstance.transform);

                instantiatedContainers.Add(newContainerInstance);

                GameObject containerParent = new GameObject($"Container ({(i + 1)})");
                containerParent.transform.SetParent(objHolder, false);
                newContainerInstance.ContainerParent = containerParent;

                containerParent.transform.position =
                    gameModeData.leftmostContainerPositions[containerToSpawn - 1] +
                    (i * distanceBetweenContainers);
                containerParent.transform.localScale =
                    Vector3.one * gameModeData.containerGeneralScaling[containerToSpawn - 1];
            }

            return instantiatedContainers;
        }

        public static void SetContainersParameters(List<ContainerInstance> containers, GameModeData gameModeData)
        {
            for (int i = 0; i < containers.Count; ++i)
                SetContainerParameters(containers[i], gameModeData.skinData.playersSkinData[i]);
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

        public static List<CannonInstance> InstantiateCannons(int playerCount, GameModeData gameModeData,
            List<ContainerInstance> containers)
        {
            if (!containers.Any() || playerCount <= 0)
                return null;

            List<CannonInstance> instantiatedCannons = new List<CannonInstance>();

            for (int i = 0; i < playerCount; i++)
            {
                ContainerInstance cannonContainerInstance =
                    containers[GetContainerIndexForPlayer(i, gameModeData.playerPerContainer)];
                instantiatedCannons.Add(InstantiateCannon(gameModeData, cannonContainerInstance));
            }

            return instantiatedCannons;
        }

        public static CannonInstance InstantiateCannon(GameModeData gameModeData, ContainerInstance containerInstance)
        {
            var newCannon = Object.Instantiate(gameModeData.cannonInstancePrefab,
                containerInstance.ContainerParent.transform);
            ResetLocalTransform(newCannon.transform);

            float xPos = gameModeData.isCannonSpawnXPosRandom
                ? Random.Range(-containerInstance.GetContainerHorizontalHalfLength(),
                    containerInstance.GetContainerHorizontalHalfLength())
                : 0f;
            newCannon.transform.localPosition = new Vector2(xPos, gameModeData.cannonVerticalDistanceFromCenter);

            return newCannon;
        }

        public static void SetCannonsParameters(List<CannonInstance> cannons, List<ContainerInstance> containers,
            GameModeData gameModeData)
        {
            for (int i = 0; i < cannons.Count; ++i)
            {
                SetCannonParameters(i, cannons[i],
                    containers[GetContainerIndexForPlayer(i, gameModeData.playerPerContainer)],
                    gameModeData, gameModeData.skinData.playersSkinData[i]);
            }
        }

        public static void SetCannonParameters(int playerIndex, CannonInstance cannonInstance, ContainerInstance containerInstance,
            GameModeData gameModeData, PlayerSkinData playerSkinData)
        {
            cannonInstance.SetPlayerIndex(playerIndex);
            
            cannonInstance.speed = gameModeData.cannonSpeed;
            cannonInstance.reloadCooldown = gameModeData.cannonReloadCooldown;
            cannonInstance.shootingForce = gameModeData.cannonShootingForce;
            cannonInstance.emptyDistanceBetweenBallAndCannon = gameModeData.emptyDistanceBetweenBallAndCannon;
            cannonInstance.isUsingPeggleMode = gameModeData.isCannonUsingPeggleMode;
            cannonInstance.horizontalMargin = containerInstance.GetContainerHorizontalHalfLength();

            cannonInstance.ballSetData = gameModeData.ballSetData;
            cannonInstance.ballSpriteData = playerSkinData.ballTheme;
            // cannonInstance.scoreReference = playerScoreRef;
            cannonInstance.containerInstance = containerInstance;
            cannonInstance.spriteRenderer.sprite = playerSkinData.cannonSprite;
        }

        // public static void ConnectCannonsToPlayerInputs(List<CannonInstance> cannons,
        //     List<PlayerInputSystem> playerInputHandlers)
        // {
        //     for (int i = 0; i < cannons.Count; ++i)
        //     {
        //         cannons[i].ConnectCannonToPlayer(playerInputHandlers[i]);
        //     }
        // }

        #endregion

        #region Ball

        public static BallInstance InstantiateBall(BallSetData ballSetData, ContainerInstance containerInstance,
            Vector3 position, float randomRotationRange = 35f)
        {
            var newBall = Object.Instantiate(ballSetData.ballInstancePrefab,
                containerInstance.ContainerParent.transform);
            ResetLocalTransform(newBall.transform);

            newBall.transform.SetLocalPositionAndRotation(position,
                Quaternion.Euler(0f, 0f, Random.Range(-randomRotationRange, randomRotationRange)));

            return newBall;
        }

        // public static void SetBallParameters(BallInstance ballInstance, int ballTierIndex, IntReference scoreRef,
        //     BallSetData ballSetData, BallTracker ballTracker, BallSpriteThemeData ballSpriteThemeData,
        //     ContainerInstance containerInstance, IGameModeManager gameModeManager, bool disableCollision = false)
        // {
        //     // var ballData = ballSetData.GetBallData(ballTierIndex);
        //     // if (ballData == null)
        //     // {
        //     //     Debug.LogError("Trying to spawn a ball with a tier that doesn't exist");
        //     //     Object.Destroy(ballInstance.gameObject);
        //     // }
        //
        //     // ballInstance.spriteRenderer.sprite = ballSpriteThemeData.ballSprites[ballTierIndex];
        //     
        //     // Tout ça (pos et scale) devrait aller dans le init
        //     
        //     // var ballTransform = ballInstance.transform;
        //     // ballTransform.localScale = Vector3.one * ballData.scale;
        //     // ballTransform.name = $"Ball T{ballInstance.tier} (ID: {ballInstance.transform.GetInstanceID()})";
        //
        //     
        //     
        //     // ballInstance.rb2d.mass = ballData.mass;
        //     // var ballPhysMat = new PhysicsMaterial2D("ballPhysMat")
        //     // {
        //     //     bounciness = ballSetData.bounciness,
        //     //     friction = ballSetData.friction
        //     // };
        //     // ballInstance.rb2d.sharedMaterial = ballPhysMat;
        //
        //     // ballInstance.tier = ballData.index;
        //     // ballInstance.scoreValue = ballData.GetScoreValue();
        //     ballInstance.ballScoreRef = scoreRef;
        //     // ballInstance.ballSetData = ballSetData;
        //     // ballInstance.ballSpriteThemeData = ballSpriteThemeData;
        //     // ballInstance.containerInstance = containerInstance;
        //     ballInstance.ballTracker = ballTracker;
        //
        //     // ballInstance.impulseMultiplier = ballSetData.impulseForcePerUnit;
        //     // ballInstance.impulseExpPower = ballSetData.impulseExpPower;
        //     // ballInstance.impulseRangeMultiplier = ballSetData.impulseRangeMultiplier;
        //
        //
        //     if (disableCollision)
        //         ballInstance.rb2d.simulated = false;
        // }

        #endregion

        public static Vector3 WorldToLocalPosition(Transform relativeTargetTransform, Vector3 worldPosition) =>
            relativeTargetTransform.InverseTransformPoint(worldPosition);

        private static Transform GetObjectsHolder()
        {
            var objects = GameObject.Find("Objects");
            if (objects == null)
                objects = new GameObject($"Objects");
            return objects.transform;
        }

        public static int GetContainerIndexForPlayer(int playerIndex, int playerPerContainer) =>
            DivideIntRoundedUp(playerIndex + 1, playerPerContainer) - 1;

        private static int DivideIntRoundedUp(int a, int b) => a / b + (a % b > 0 ? 1 : 0);

        private static void ResetLocalTransform(Transform child)
        {
            child.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            child.localScale = Vector3.one;
        }
    }
}