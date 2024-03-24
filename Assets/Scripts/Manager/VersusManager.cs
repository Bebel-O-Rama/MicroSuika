using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.Manager
{
    public class VersusManager : MonoBehaviour
    {
        #region Singleton

        public static VersusManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            Initialize();
        }

        #endregion

        [FormerlySerializedAs("versusData")] [FormerlySerializedAs("_winConditionData")] [SerializeField]
        private VersusWinConditionData versusWinConditionData;


        [Header("GameMode Parameters")] [SerializeField]
        public GameModeData gameModeData;


        private FloatReference _averageSpeed;


        private int _playerIndexInLead;
        private FloatReference _currentLeadTimeCondition;
        private FloatReference _currentLeadSpeedCondition;
        private float _leadRequirementProgressionTime;

        private Coroutine _leadTimerCoroutine;


        private bool _isGameInProgress = true;


        public ActionMethodPlayerWrapper<float> OnLeadStart { get; } = new ActionMethodPlayerWrapper<float>();
        public ActionMethodPlayerWrapper<bool> OnLeadStop { get; } = new ActionMethodPlayerWrapper<bool>();


        private void Initialize()
        {
            _currentLeadTimeCondition = new FloatReference();
            _currentLeadSpeedCondition = new FloatReference();
        }

        private void Start()
        {
            SpawnContainersVersus();
            SpawnCannonsVersus();
            
            // SetContainerRacingParameters();
        }

        private void Update()
        {
            _leadRequirementProgressionTime += Time.deltaTime;

            if (!_isGameInProgress)
                return;
            var currentPlayerRankings = ScoreManager.Instance.GetPlayerRankings();
            if (!CheckLeadConditions(currentPlayerRankings))
            {
                UpdateLeadRequirementsParameters();
                StopLeadTimer();
                return;
            }

            if (currentPlayerRankings.First() == _playerIndexInLead)
                return;

            StopLeadTimer();
            StartLeadTimer(currentPlayerRankings.First());
        }

        private void SpawnContainersVersus()
        {
            var objHolder = GameObject.Find("Objects");
            if (!objHolder)
            {
                objHolder = new GameObject("Objects");
            }
            
            // Get the number of container to instantiate
            int numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();
            if (numberOfActivePlayer <= 0)
                return;

            var containersToSpawn =
                UnityExtension.DivideIntRoundedUp(numberOfActivePlayer, gameModeData.PlayerPerContainer);

            for (int i = 0; i < containersToSpawn; i++)
            {
                var containerParent = new GameObject($"Container ({(i + 1)})");
                containerParent.transform.SetParent(objHolder.transform, false);
                
                var container = Instantiate(gameModeData.ContainerInstancePrefab, containerParent.transform);

                // Quick workaround to get the list of playerIndex for the container before we add it to the Tracker
                var playerIndexes = new List<int>();
                for (int j = i * gameModeData.PlayerPerContainer;
                     j < Mathf.Min(i + gameModeData.PlayerPerContainer, numberOfActivePlayer);
                     j++)
                {
                    playerIndexes.Add(j);
                }

                ContainerTracker.Instance.AddNewItem(container, playerIndexes);
                container.SetContainerParameters(gameModeData, i, containersToSpawn);
            }
        }

        private void SpawnCannonsVersus()
        {
            int numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();
            for (int i = 0; i < numberOfActivePlayer; i++)
            {
                var container = ContainerTracker.Instance.GetItemsByPlayer(i)[0];
                var cannon = Instantiate(gameModeData.CannonInstancePrefab, container.ContainerParent.transform);
                CannonTracker.Instance.AddNewItem(cannon, i);

                cannon.SetCannonParameters(i, gameModeData);
                cannon.SetInputParameters(PlayerManager.Instance.GetPlayerInputHandler(i));
                cannon.SetCannonInputEnabled(true);
            }
        }


        #region LeadCondition

        private bool CheckLeadConditions(List<int> currentPlayerRankings)
        {
            var firstPlayerSpeed = ScoreManager.Instance.GetCurrentSpeedReference(currentPlayerRankings.First());
            return versusWinConditionData.SpeedEvaluationMethod switch
            {
                LeadSpeedEvaluationMethod.FromAverage => firstPlayerSpeed > _averageSpeed + _currentLeadSpeedCondition,
                LeadSpeedEvaluationMethod.FromSecond => firstPlayerSpeed > (currentPlayerRankings.Count > 1
                    ? ScoreManager.Instance.GetCurrentSpeedReference(currentPlayerRankings[1]) +
                      _currentLeadSpeedCondition
                    : _currentLeadSpeedCondition),
                LeadSpeedEvaluationMethod.FromStart => firstPlayerSpeed > _currentLeadSpeedCondition,
                _ => false
            };
        }

        private void UpdateLeadRequirementsParameters()
        {
            if (versusWinConditionData.AdaptiveRequirementMethod == LeadAdaptiveRequirementMethod.Fixed)
                return;

            _currentLeadTimeCondition.Variable.SetValue(
                versusWinConditionData.TimeProgressionCurve.Evaluate(Mathf.Clamp01(_leadRequirementProgressionTime /
                    versusWinConditionData.TimeProgressionLength)) *
                versusWinConditionData.TimeRequirementRange.y + versusWinConditionData.TimeRequirementRange.x);
            _currentLeadSpeedCondition.Variable.SetValue(
                versusWinConditionData.SpeedProgressionCurve.Evaluate(Mathf.Clamp01(_leadRequirementProgressionTime /
                    versusWinConditionData.SpeedProgressionLength)) *
                versusWinConditionData.SpeedRequirementRange.y + versusWinConditionData.SpeedRequirementRange.x);
        }

        private void StartLeadTimer(int playerIndex)
        {
            _playerIndexInLead = playerIndex;
            OnLeadStart.CallAction(_currentLeadTimeCondition, _playerIndexInLead);
            _leadTimerCoroutine = StartCoroutine(LeadTimer());
        }

        private IEnumerator LeadTimer()
        {
            yield return new WaitForSeconds(_currentLeadTimeCondition);
            ProcessGameOver();
        }

        private void StopLeadTimer()
        {
            if (_leadTimerCoroutine != null)
            {
                StopCoroutine(_leadTimerCoroutine);
                OnLeadStop.CallAction(false, _playerIndexInLead);
            }

            _playerIndexInLead = -1;
        }

        #endregion

        #region GameOver

        // It would be nicer if the main objects (Containers and Cannons) could subscribe to an Action and
        // be directly called when the game is over. We could also pass the inState as parameter.
        private void ProcessGameOver()
        {
            var winnerPlayerIndex = ScoreManager.Instance.GetPlayerRankings().First();
            foreach (var cannon in CannonTracker.Instance.GetItems())
                cannon.DisconnectCannonFromPlayer();

            var winnerContainer = ContainerTracker.Instance.GetItemsByPlayer(winnerPlayerIndex).First();

            foreach (var container in ContainerTracker.Instance.GetItems())
            {
                container.OnGameOver(container == winnerContainer);
            }

            foreach (var ball in BallTracker.Instance.GetItems())
            {
                ball.SetSimulatedParameters(false);
            }

            _isGameInProgress = false;
        }

        #endregion

        // private void SetContainerRacingParameters()
        // {
        //     // TODO: clean up this, it's for a quick test
        //     var playerIndex = 1;
        //     foreach (var container in ContainerTracker.Instance.GetItems())
        //     {
        //         var containerRacing = container.GetComponent<ContainerRacingMode>();
        //         containerRacing.SetLayer($"Container{playerIndex}");
        //         playerIndex++;
        //     }
        // }

        public (FloatReference timeRequirement, FloatReference speedRequirement) GetLeadRequirementReferences()
        {
            return (_currentLeadTimeCondition, _currentLeadSpeedCondition);
        }
    }
}