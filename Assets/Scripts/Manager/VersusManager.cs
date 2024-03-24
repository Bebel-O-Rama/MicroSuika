using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using MultiSuika.zOther;
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

        [Header("Speed Parameters")] [SerializeField]
        private FloatReference _speedSoftCap; // 1200

        [Header("Ball Collision Parameters")] [SerializeField]
        private FloatReference _ballImpactMultiplier; // 2 

        [FormerlySerializedAs("versusData")] [FormerlySerializedAs("_winConditionData")] [SerializeField]
        private VersusWinConditionData versusWinConditionData;

        [Header("Movement Parameters")] [SerializeField]
        private FloatReference _minAdaptiveVerticalRange;

        [Header("GameMode Parameters")] [SerializeField]
        public GameModeData gameModeData;

        // [Header("DEBUG parameters")] public bool useDebugSpawnContainer = false;
        // [Range(0, 4)] [SerializeField] public int debugFakeNumberCount = 2;
        // [SerializeField] public bool checkLeadCondition = true;
        // [SerializeField] private BoolReference _isRacingModeDebugTextEnabled;
        // [SerializeField] private BoolReference _isContainerSpeedBarDebugEnabled;
        // [SerializeField] private BoolReference _isContainerFullDebugTextEnabled;
        // [SerializeField] private BoolReference _isContainerAbridgedDebugTextEnabled;
        // [SerializeField] private FloatReference _debugScoreMultiplier;

        // private Dictionary<ContainerInstance, FloatReference> _playerCurrentSpeedReferences;
        // private Dictionary<ContainerInstance, IntReference> _playerRankingReferences;
        // private Dictionary<ContainerInstance, BoolReference> _playerLeadStatus;
        // private Dictionary<ContainerInstance, FloatReference> _playerLeadTimer;
        // private Dictionary<ContainerInstance, FloatReference> _playersYPositionRatio;

        private FloatReference _averageSpeed;
        // private FloatReference _firstPlayerSpeed;
        // private FloatReference _lastPlayerSpeed;
        // private FloatReference _standardDeviationSpeed;

        private int _playerIndexInLead;
        private FloatReference _currentLeadTimeCondition;
        private FloatReference _currentLeadSpeedCondition;
        private float _leadRequirementProgressionTime;

        private Coroutine _leadTimerCoroutine;
        // private float _currentLeadTimeLeft;
        // private ContainerInstance _currentContainerInstanceInLead;
        // private float _timeReqProgressionTimer;
        // private float _speedReqProgressionTimer;

        // private IntReference _dampingMethodIndex;


        private bool _isGameInProgress = true;
        // private RacingModeDebugInfo _racingModeDebugInfo;


        public ActionMethodPlayerWrapper<float> OnLeadStart { get; } = new ActionMethodPlayerWrapper<float>();
        public ActionMethodPlayerWrapper<bool> OnLeadStop { get; } = new ActionMethodPlayerWrapper<bool>();


        private void Initialize()
        {
        }

        private void Start()
        {
            int numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();

            var containers =
                Initializer.InstantiateContainers(numberOfActivePlayer, gameModeData);
            Initializer.SetContainersParameters(containers, gameModeData);

            // TEMP STUFF BEFORE I TWEAK THE CONTAINERS
            foreach (var container in containers)
            {
                ContainerTracker.Instance.AddNewItem(container);
            }

            for (int i = 0; i < numberOfActivePlayer; i++)
            {
                ContainerTracker.Instance.SetPlayerForItem(i,
                    containers[UnityExtensions.DivideIntRoundedUp(i + 1, gameModeData.PlayerPerContainer) - 1]);
            }


            //// Instantiate and Set CannonInstances
            for (int i = 0; i < numberOfActivePlayer; i++)
            {
                var container = ContainerTracker.Instance.GetItemsByPlayer(i)[0];
                var cannon = Instantiate(gameModeData.CannonInstancePrefab, container.ContainerParent.transform);

                cannon.SetCannonParameters(i, gameModeData);
                cannon.SetInputParameters(PlayerManager.Instance.GetPlayerInputHandler(i));
                cannon.SetCannonInputEnabled(true);
                CannonTracker.Instance.AddNewItem(cannon, i);
            }

            //// Racing Stuff!!!
            SetRacingModeParameters();
            SetContainerRacingParameters();
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
            ProcessWinCondition();
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


        // TODO: Have the main objects (container, cannon) subscribe to an Action that will call the gameOver
        private void ProcessWinCondition()
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

        private void SetRacingModeParameters()
        {
            _currentLeadTimeCondition = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _currentLeadSpeedCondition = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
        }

        private void SetContainerRacingParameters()
        {
            // TODO: clean up this, it's for a quick test
            var playerIndex = 1;
            foreach (var container in ContainerTracker.Instance.GetItems())
            {
                var containerRacing = container.GetComponent<ContainerRacingMode>();
                containerRacing.SetLayer($"Container{playerIndex}");
                playerIndex++;
            }
        }

        public (FloatReference timeRequirement, FloatReference speedRequirement) GetLeadRequirementReferences()
        {
            return (_currentLeadTimeCondition, _currentLeadSpeedCondition);
        }
    }
}