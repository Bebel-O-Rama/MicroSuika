using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.DebugInfo;
using MultiSuika.Manager;
using MultiSuika.ScoreSystemTransition;
using MultiSuika.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.GameLogic
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

        [Header("Speed Parameters")] 
        [SerializeField] private FloatReference _speedSoftCap; // 1200

        [Header("Ball Collision Parameters")] 
        [SerializeField] private FloatReference _ballImpactMultiplier; // 2 
        
        [SerializeField] private WinConditionData _winConditionData;
        
        [Header("Movement Parameters")] 
        [SerializeField] private FloatReference _minAdaptiveVerticalRange;

        [Header("GameMode Parameters")] 
        [SerializeField] public GameModeData gameModeData;

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
                    containers[Initializer.GetContainerIndexForPlayer(i, gameModeData.playerPerContainer)]);
            }


            //// Init and set cannons
            for (int i = 0; i < numberOfActivePlayer; i++)
            {
                var container = ContainerTracker.Instance.GetItemsByPlayer(i)[0];
                var cannon = Initializer.InstantiateCannon(gameModeData, container);
                CannonTracker.Instance.AddNewItem(cannon, i);

                Initializer.SetCannonParameters(i, cannon, container, gameModeData, gameModeData.skinData.playersSkinData[i]);
                cannon.SetInputParameters(PlayerManager.Instance.GetPlayerInputHandler(i));
                cannon.SetCannonInputEnabled(true);
            }

            //// Racing Stuff!!!
            SetRacingModeParameters();
            SetContainerRacingParameters();
            

            // SetRacingModeDebugInfoParameters();
            // _racingModeDebugInfo.SetDebugActivationParameters(_isRacingModeDebugTextEnabled);

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
            return _winConditionData.SpeedEvaluationMethod switch
            {
                LeadSpeedEvaluationMethod.FromAverage => firstPlayerSpeed > _averageSpeed + _currentLeadSpeedCondition,
                LeadSpeedEvaluationMethod.FromSecond => firstPlayerSpeed > (currentPlayerRankings.Count > 1
                    ? ScoreManager.Instance.GetCurrentSpeedReference(currentPlayerRankings[1]) + _currentLeadSpeedCondition
                    : _currentLeadSpeedCondition),
                LeadSpeedEvaluationMethod.FromStart => firstPlayerSpeed > _currentLeadSpeedCondition,
                _ => false
            };
        }
        
        private void UpdateLeadRequirementsParameters()
        {
            if (_winConditionData.AdaptiveRequirementMethod == LeadAdaptiveRequirementMethod.Fixed)
                return;

            _currentLeadTimeCondition.Variable.SetValue(
                _winConditionData.TimeProgressionCurve.Evaluate(Mathf.Clamp01(_leadRequirementProgressionTime / _winConditionData.TimeProgressionLength)) *
                _winConditionData.TimeRequirementRange.y + _winConditionData.TimeRequirementRange.x);
            _currentLeadSpeedCondition.Variable.SetValue(
                _winConditionData.SpeedProgressionCurve.Evaluate(Mathf.Clamp01(_leadRequirementProgressionTime / _winConditionData.SpeedProgressionLength)) *
                _winConditionData.SpeedRequirementRange.y + _winConditionData.SpeedRequirementRange.x);
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
                cannon.DisconnectCannonToPlayer();

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
        
        // private void SetRacingModeDebugInfoParameters()
        // {
        //     _racingModeDebugInfo = FindObjectOfType<RacingModeDebugInfo>();
        //     if (_racingModeDebugInfo == null)
        //         return;
        //     _racingModeDebugInfo.SetRacingModeParameters(_averageSpeed, _standardDeviationSpeed,
        //         _currentLeadTimeCondition, _currentLeadSpeedCondition);
        //     _racingModeDebugInfo.SetDebugActivationParameters(_isRacingModeDebugTextEnabled);
        // }

        private void SetRacingModeParameters()
        {
            // _firstPlayerSpeed = new FloatReference
            //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            // _lastPlayerSpeed = new FloatReference
            //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            // _standardDeviationSpeed = new FloatReference
            //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _currentLeadTimeCondition = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _currentLeadSpeedCondition = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            // _dampingMethodIndex = new IntReference
            //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
        
            // _playerCurrentSpeedReferences = new Dictionary<ContainerInstance, FloatReference>();
            // _playerRankingReferences = new Dictionary<ContainerInstance, IntReference>();
            // _playerLeadStatus = new Dictionary<ContainerInstance, BoolReference>();
            // _playerLeadTimer = new Dictionary<ContainerInstance, FloatReference>();
            // _playersYPositionRatio = new Dictionary<ContainerInstance, FloatReference>();
        
            // foreach (var container in ContainerTracker.Instance.GetItems())
            // {
            //     // FloatReference newCurrentSpeedVar = new FloatReference
            //     //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            //     // IntReference newPlayerRankingRef = new IntReference
            //     //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
            //     // BoolReference newPlayerLeadStatus = new BoolReference
            //     //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<BoolVariable>() };
            //     // FloatReference newPlayerLeadTimer = new FloatReference
            //     //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            //     // var newYPositionRatioRef = new FloatReference
            //     //     { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            //
            //     // newPlayerLeadStatus.Variable.SetValue(false);
            //     //
            //     // _playerCurrentSpeedReferences[container] = newCurrentSpeedVar;
            //     // _playerRankingReferences[container] = newPlayerRankingRef;
            //     // _playerLeadStatus[container] = newPlayerLeadStatus;
            //     // _playerLeadTimer[container] = newPlayerLeadTimer;
            //     // _playersYPositionRatio[container] = newYPositionRatioRef;
            // }
        
            // _dampingMethodIndex.Variable.SetValue((int)_dampingMethod);
        
            // _currentLeadTimeCondition.Variable.SetValue(_timeLeadConditionMinRange.x + _timeLeadConditionMinRange.y);
            // _currentLeadSpeedCondition.Variable.SetValue(_speedLeadConditionMinRange.x + _speedLeadConditionMinRange.y);
        }

        private void SetContainerRacingParameters()
        {
            // TODO: clean up this, it's for a quick test
            var playerIndex = 1;
            foreach (var container in ContainerTracker.Instance.GetItems())
            {
                var containerRacing = container.GetComponent<ContainerRacingMode>();
                // // containerRacing.SetAreaParameters(_containerMaxArea);
                // containerRacing.SetSpeedParameters(_playerCurrentSpeedReferences[container], _averageSpeed,
                //     _speedSoftCap, _firstPlayerSpeed, _lastPlayerSpeed);
                // containerRacing.SetDampingParameters(_dampingMethodIndex, _dampingFixedPercent, _dampingFixedValue,
                //     _dampingCurvePercent);
                // containerRacing.SetComboParameters(_comboTimerFull, _acceleration);
                // containerRacing.SetRankingParameters(_playerRankingReferences[container]);
                // containerRacing.SetLeadParameters(_playerLeadStatus[container], _playerLeadTimer[container]);
                // containerRacing.SetCollisionParameters(_ballImpactMultiplier);
                // containerRacing.SetPositionParameters(_playersYPositionRatio[container], _minAdaptiveVerticalRange);
                // containerRacing.SetDebugActivationParameters(_isContainerSpeedBarDebugEnabled,
                    // _isContainerFullDebugTextEnabled, _isContainerAbridgedDebugTextEnabled, _debugScoreMultiplier);
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