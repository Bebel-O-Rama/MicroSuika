using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
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
        #region Player

        public static List<PlayerInputHandler> InstantiatePlayerInputHandlers(List<PlayerData> connectedPlayerData, GameModeData gameModeData)
        {
            List<PlayerInputHandler> instantiatedPlayerInputHandlers = new List<PlayerInputHandler>();
            foreach (var playerData in connectedPlayerData)
            {
                var playerInputObj = PlayerInput.Instantiate(gameModeData.playerPrefab, playerData.playerIndexNumber,
                    pairWithDevice: playerData.inputDevice);
                instantiatedPlayerInputHandlers.Add(playerInputObj.GetComponentInParent<PlayerInputHandler>());
            }

            return instantiatedPlayerInputHandlers;
        }

        #endregion

        #region Container

        public static List<Container.Container> InstantiateContainers(int playerCount,
            GameModeData gameModeData, Transform gameModeParent = null)
        {
            playerCount = playerCount <= 0 ? 1 : playerCount; // For cases like the lobby
            int containerToSpawn = DivideIntRoundedUp(playerCount, gameModeData.playerPerContainer);
            if (containerToSpawn <= 0)
                return null;

            List<Container.Container> instantiatedContainers = new List<Container.Container>();
            Vector2 distanceBetweenContainers = Vector2.zero;

            distanceBetweenContainers.x = (containerToSpawn > 1)
                ? Mathf.Abs(gameModeData.leftmostContainerPositions[containerToSpawn - 1].x) * 2f /
                  (containerToSpawn - 1)
                : 0f;

            for (int i = 0; i < containerToSpawn; i++)
            {
                GameObject containerObj = Object.Instantiate(gameModeData.containerPrefab);
                ResetLocalTransform(containerObj.transform);

                Container.Container newContainer = containerObj.GetComponent<Container.Container>();
                instantiatedContainers.Add(newContainer);

                GameObject containerParent = new GameObject($"{gameModeData.containerParentName}_{(i + 1)}");
                if (gameModeParent != null)
                    containerParent.transform.SetParent(gameModeParent, false);
                newContainer.ContainerParent = containerParent;

                containerParent.transform.position =
                    gameModeData.leftmostContainerPositions[containerToSpawn - 1] +
                    (i * distanceBetweenContainers);
                containerParent.transform.localScale =
                    Vector3.one * gameModeData.containerGeneralScaling[containerToSpawn - 1];
            }

            return instantiatedContainers;
        }

        public static void SetContainersParameters(List<Container.Container> containers, GameModeData gameModeData)
        {
            for (int i = 0; i < containers.Count; ++i)
                SetContainerParameters(containers[i], gameModeData.skinData.playersSkinData[i]);
        }
    
        private static void SetContainerParameters(Container.Container container, PlayerSkinData playerSkinData)
        {
            container.backgroundSpriteRenderer.sprite = playerSkinData.containerBackground;
            container.sideSpriteRenderer.sprite = playerSkinData.containerSide;
            container.failureSpriteRenderer.sprite = playerSkinData.containerFailure;
            container.successSpriteRenderer.sprite = playerSkinData.containerSuccess;
        }

        #endregion

        #region Cannon

        public static List<Cannon.Cannon> InstantiateCannons(int playerCount, GameModeData gameModeData,
            List<Container.Container> containers)
        {
            if (!containers.Any() || playerCount <= 0)
                return null;

            List<Cannon.Cannon> instantiatedCannons = new List<Cannon.Cannon>();

            for (int i = 0; i < playerCount; i++)
            {
                Container.Container cannonContainer = containers[GetContainerIndexForPlayer(i, gameModeData.playerPerContainer)];
                instantiatedCannons.Add(InstantiateCannon(gameModeData, cannonContainer));
            }

            return instantiatedCannons;
        }

        public static Cannon.Cannon InstantiateCannon(GameModeData gameModeData, Container.Container container)
        {
            GameObject cannonObj = Object.Instantiate(gameModeData.cannonPrefab, container.ContainerParent.transform);
            ResetLocalTransform(cannonObj.transform);

            float xPos = gameModeData.isCannonSpawnXPosRandom
                ? Random.Range(-container.GetContainerHorizontalHalfLength(),
                    container.GetContainerHorizontalHalfLength())
                : 0f;
            cannonObj.transform.localPosition = new Vector2(xPos, gameModeData.cannonVerticalDistanceFromCenter);

            return cannonObj.GetComponent<Cannon.Cannon>();
        }

        public static void SetCannonsParameters(List<Cannon.Cannon> cannons, List<Container.Container> containers, BallTracker balltracker, GameModeData gameModeData,
            List<PlayerData> playerData)
        {
            for (int i = 0; i < cannons.Count; ++i)
            {
                SetCannonParameters(cannons[i], containers[GetContainerIndexForPlayer(i, gameModeData.playerPerContainer)], balltracker,
                    gameModeData, playerData[i], gameModeData.skinData.playersSkinData[i]);
            }
        }

        public static void SetCannonParameters(Cannon.Cannon cannon, Container.Container container, BallTracker ballTracker, GameModeData gameModeData,
            PlayerData playerData, PlayerSkinData playerSkinData)
        {
            cannon.speed = gameModeData.cannonSpeed;
            cannon.reloadCooldown = gameModeData.cannonReloadCooldown;
            cannon.shootingForce = gameModeData.cannonShootingForce;
            cannon.emptyDistanceBetweenBallAndCannon = gameModeData.emptyDistanceBetweenBallAndCannon;
            cannon.isUsingPeggleMode = gameModeData.isCannonUsingPeggleMode;
            cannon.horizontalMargin = container.GetContainerHorizontalHalfLength();

            cannon.ballSetData = gameModeData.ballSetData;
            cannon.ballSpriteData = playerSkinData.ballTheme;
            cannon.scoreReference = playerData.mainScore;
            cannon.container = container;
            cannon.ballTracker = ballTracker;
            cannon.spriteRenderer.sprite = playerSkinData.cannonSprite;
        }

        public static void ConnectCannonsToPlayerInputs(List<Cannon.Cannon> cannons, List<PlayerInputHandler> playerInputHandlers)
        {
            for (int i = 0; i < cannons.Count; ++i)
            {
                cannons[i].ConnectCannonToPlayer(playerInputHandlers[i]);
            }
        }
        
        #endregion

        #region Ball

        public static Ball.Ball InstantiateBall(BallSetData ballSetData, Container.Container container,
            Vector3 position, float randomRotationRange = 35f)
        {
            var ballObj = Object.Instantiate(ballSetData.ballPrefab, container.ContainerParent.transform);
            ResetLocalTransform(ballObj.transform);

            ballObj.transform.localPosition = position;
            ballObj.transform.localRotation =
                Quaternion.Euler(0f, 0f, Random.Range(-randomRotationRange, randomRotationRange));
        
            return ballObj.GetComponent<Ball.Ball>();
        }

        public static void SetBallParameters(Ball.Ball ball, int ballTierIndex, IntReference scoreRef, BallSetData ballSetData, BallTracker ballTracker, BallSpriteThemeData ballSpriteThemeData, Container.Container container, bool disableCollision = false)
        {
            var ballData = ballSetData.GetBallData(ballTierIndex);
            if (ballData == null)
            {
                Debug.LogError("Trying to spawn a ball with a tier that doesn't exist");
                Object.Destroy(ball.gameObject);
            }
        
            ball.spriteRenderer.sprite = ballSpriteThemeData.ballSprites[ballTierIndex];
            ball.transform.localScale = Vector3.one * ballData.scale;
        
            ball.rb2d.mass = ballData.mass;
            var ballPhysMat = new PhysicsMaterial2D("ballPhysMat")
            {
                bounciness = ballSetData.bounciness,
                friction = ballSetData.friction
            };
            ball.rb2d.sharedMaterial = ballPhysMat;

            ball.tier = ballData.index;
            ball.scoreValue = ballData.GetScoreValue();
            ball.ballScoreRef = scoreRef;
            ball.ballSetData = ballSetData;
            ball.ballSpriteThemeData = ballSpriteThemeData;
            ball.container = container;
            ball.ballTracker = ballTracker;

            ball.impulseMultiplier = ballSetData.impulseMultiplier;
            ball.impulseExpPower = ballSetData.impulseExpPower;
            ball.impulseRangeMultiplier = ballSetData.impulseRangeMultiplier;
        

            if (disableCollision)
                ball.rb2d.simulated = false;
        }

        #endregion

        private static int GetContainerIndexForPlayer(int playerIndex, int playerPerContainer) =>
            DivideIntRoundedUp(playerIndex + 1, playerPerContainer) - 1;

        private static int DivideIntRoundedUp(int a, int b) => a / b + (a % b > 0 ? 1 : 0);

        private static void ResetLocalTransform(Transform child)
        {
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
        }

        public static Vector3 WorldToLocalPosition(Transform relativeTargetTransform, Vector3 worldPosition) =>
            relativeTargetTransform.InverseTransformPoint(worldPosition);
    }
}