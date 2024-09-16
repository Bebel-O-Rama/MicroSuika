using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
using MultiSuika.UI;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Manager
{
    public class VersusManager : MonoBehaviour
    {
        #region Singleton

        public static VersusManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);

            Initialize();
        }

        #endregion

        [SerializeField] public GameModeData gameModeData;
        [SerializeField] private VersusWinConditionData versusWinConditionData;
        [SerializeField] private float _delayBeforeGameReset;
        
        private bool _isGameInProgress = true;
        private FloatReference _averageSpeed;

        // GameScoreTransition
        [SerializeField] private ScoreTransitionVersus _scoreTransitionVersus;
        
        // Lead parameters
        [SerializeField] private int _playerIndexInLead;
        [SerializeField] private FloatReference _currentLeadTimeCondition;
        [SerializeField] private FloatReference _currentLeadSpeedCondition;
        [SerializeField] private float _leadRequirementProgressionTime;
        private Coroutine _leadTimerCoroutine;

        public ActionMethodPlayerWrapper<float> OnLeadStart { get; } = new ActionMethodPlayerWrapper<float>();
        public ActionMethodPlayerWrapper<bool> OnLeadStop { get; } = new ActionMethodPlayerWrapper<bool>();
        public ActionMethodPlayerWrapper<bool> OnGameOver { get; } = new ActionMethodPlayerWrapper<bool>();


        private void Initialize()
        {
            _currentLeadTimeCondition = new FloatReference();
            _currentLeadSpeedCondition = new FloatReference();
        }

        private void Start()
        {
            SpawnContainersVersus();
            SpawnCannonsVersus();
        }

        private void Update()
        {
            _leadRequirementProgressionTime += Time.deltaTime;

            if (!_isGameInProgress)
                return;
            var currentPlayerRankings = ScoreManager.Instance.GetPlayerSpeedRankings();
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

        public void ResetGame()
        {
            ScoreManager.Instance.ResetSpeedInformation();

            BallTracker.Instance.ClearItems();
            CannonTracker.Instance.ClearItems();
            ContainerTracker.Instance.ClearItems();

            OnLeadStart.ClearAll();
            OnLeadStop.ClearAll();
            OnGameOver.ClearAll();
            
            SpawnContainersVersus();
            SpawnCannonsVersus();
            

            ResetLeadParameters();

            // The delay is to make sure the transition is over before we start checking every frame for a winner.
            // We might need to change this if the transition is a bit longer (animation, etc.)
            Invoke("EnableGameProgress", 0.5f);
        }

        private void EnableGameProgress() => _isGameInProgress = true;
        
        #region Spawner

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

            for (int i = 0; i < numberOfActivePlayer; i++)
            {
                var containerParent = GameObject.Find($"Container ({(i + 1)})") ?? new GameObject($"Container ({(i + 1)})");
                containerParent.transform.SetParent(objHolder.transform, false);

                var container = Instantiate(gameModeData.ContainerInstancePrefab, containerParent.transform);

                ContainerTracker.Instance.AddNewItem(container, i);
                container.SetContainerParameters(gameModeData, i, numberOfActivePlayer);

                // Set the info in the ContainerNextBall components
                var containerNextBall = container.GetComponent<ContainerNextBall>();
                if (containerNextBall)
                {
                    containerNextBall.SetNextBallParameters(i, gameModeData);
                }
            }
        }

        private void SpawnCannonsVersus()
        {
            int numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();
            for (int i = 0; i < numberOfActivePlayer; i++)
            {
                var container = ContainerTracker.Instance.GetItemFromPlayerOrDefault(i);
                var cannon = Instantiate(gameModeData.CannonInstancePrefab, container.ContainerParent.transform);
                CannonTracker.Instance.AddNewItem(cannon, i);

                cannon.SetCannonParameters(i, gameModeData);
                cannon.SetInputParameters(PlayerManager.Instance.GetPlayerInputHandler(i));
                cannon.SetCannonInputEnabled(true);
                
                OnGameOver.Subscribe(cannon.OnGameOver, i);
            }
        }

        #endregion

        #region LeadCondition

        private bool CheckLeadConditions(List<int> currentPlayerRankings)
        {
            if (versusWinConditionData.AdaptiveRequirementMethod == LeadAdaptiveRequirementMethod.Disabled)
                return false;
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
            if (versusWinConditionData.AdaptiveRequirementMethod != LeadAdaptiveRequirementMethod.AnimCurve)
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
                // _leadTimerCoroutine = null;
                OnLeadStop.CallAction(false, _playerIndexInLead);
            }

            _playerIndexInLead = -1;
        }

        private void ResetLeadParameters()
        {
            StopLeadTimer();
            // StopCoroutine(_leadTimerCoroutine);
            _playerIndexInLead = 0;
            _leadRequirementProgressionTime = 0;
            _currentLeadTimeCondition.Variable.SetValue(0);
            _currentLeadSpeedCondition.Variable.SetValue(0);
        }

        #endregion

        #region GameOver

        // It would be nicer if the main objects (Containers and Cannons) could subscribe to an Action and
        // be directly called when the game is over. We could also pass the winState as parameter.
        private void ProcessGameOver()
        {
            var winnerPlayerIndex = ScoreManager.Instance.GetPlayerSpeedRankings().First();
            foreach (var cannon in CannonTracker.Instance.GetItems())
                cannon.SetCannonInputEnabled(false);

            for (var i = 0; i < PlayerManager.Instance.GetNumberOfActivePlayer(); i++)
            {
                OnGameOver.CallAction(i == winnerPlayerIndex, i);
            }

            foreach (var ball in BallTracker.Instance.GetItems())
            {
                ball.SetSimulatedParameters(false);
            }

            _isGameInProgress = false;
            
            // UPDATE THAT!!!
            var isTargetScoreTriggered = ScoreManager.Instance.UpdateWinnerScore(winnerPlayerIndex);

            _scoreTransitionVersus.ScoreTransitionSequence(winnerPlayerIndex, isTargetScoreTriggered);

            Invoke("ResetGame", _delayBeforeGameReset);
        }

        #endregion

        #region Getter/Setter

        public (FloatReference timeRequirement, FloatReference speedRequirement) GetLeadRequirementReferences()
        {
            return (_currentLeadTimeCondition, _currentLeadSpeedCondition);
        }

        public (Sprite outside, Material glow) GetContainerEffectAssets(int playerIndex) =>
            (gameModeData.SkinData.GetPlayerSkinData(playerIndex).ContainerOutside,
                gameModeData.SkinData.GetPlayerSkinData(playerIndex).ContainerGlow);

        #endregion
    }
}