using System;
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

namespace MultiSuika.GameLogic
{
    public class RacingModeManager : MonoBehaviour, IGameModeManager
    {
        // #region Singleton
        //
        // [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        // public static RacingModeManager Instance => _instance ??= new RacingModeManager();
        //
        // private static RacingModeManager _instance;
        //
        // private RacingModeManager()
        // {
        // }
        //
        // #endregion
        // [Header("Score Parameters")]
        [SerializeField] private ScoreHandlerData _scoreHandlerData;
        // public ScoreHandler ScoreHandler { get; private set; }

        [Header("Speed Parameters")] [SerializeField]
        private FloatReference _speedSoftCap; // 1000

        [Header("Ball Collision Parameters")] [SerializeField]
        private FloatReference _ballImpactMultiplier; // 2 

        [Header("Damping Parameters")] [SerializeField]
        private ContainerRacingMode.DampingMethod _dampingMethod; // AnimCurve

        [SerializeField] private FloatReference _dampingFixedPercent; // 0.02
        [SerializeField] private FloatReference _dampingFixedValue; // 1

        [Tooltip("The animation curve can't be modified at runtime")] [SerializeField]
        private AnimationCurve _dampingCurvePercent; // (0.0;0.0), (0.5;0.015), (1.0;0.05)

        [Header("Area Parameters")] [SerializeField]
        private FloatReference _containerMaxArea; // 60

        [Header("Combo Parameters")] [SerializeField]
        private FloatReference _comboTimerFull; // 3

        [SerializeField] private FloatReference _acceleration; // 3

        [Header("Lead Parameters")]
        [Tooltip(
            "If false, only increase the timer reducing the value of the condition variables when nobody is winning. Otherwise, always increase the timer")]
        [SerializeField]
        private bool _alwaysReduceConditionValues;

        [SerializeField] private LeadReqProgressionMethod _leadReqProgressionMethod;
        [SerializeField] private SpeedReqCheckMethod _speedReqCheckMethod;
        [SerializeField] [Min(0f)] private Vector2 _timeLeadConditionMinRange;
        [SerializeField] [Min(0f)] private Vector2 _speedLeadConditionMinRange;
        [SerializeField] [Min(1f)] private float _leadTimeConditionTimerRange;
        [SerializeField] [Min(1f)] private float _leadSpeedConditionTimerRange;
        [SerializeField] private AnimationCurve _leadTimeReqCurve;
        [SerializeField] private AnimationCurve _leadSpeedReqCurve;

        [Header("Movement Parameters")] [SerializeField]
        private FloatReference _minAdaptiveVerticalRange;

        [Header("GameMode Parameters")] [SerializeField]
        public GameModeData gameModeData;

        [Header("DEBUG parameters")] public bool useDebugSpawnContainer = false;
        [Range(0, 4)] [SerializeField] public int debugFakeNumberCount = 2;
        [SerializeField] public bool checkLeadCondition = true;
        [SerializeField] private BoolReference _isRacingModeDebugTextEnabled;
        [SerializeField] private BoolReference _isContainerSpeedBarDebugEnabled;
        [SerializeField] private BoolReference _isContainerFullDebugTextEnabled;
        [SerializeField] private BoolReference _isContainerAbridgedDebugTextEnabled;
        [SerializeField] private FloatReference _debugScoreMultiplier;
        
        private Dictionary<ContainerInstance, FloatReference> _playerCurrentSpeedReferences;
        private Dictionary<ContainerInstance, IntReference> _playerRankingReferences;
        private Dictionary<ContainerInstance, BoolReference> _playerLeadStatus;
        private Dictionary<ContainerInstance, FloatReference> _playerLeadTimer;
        private Dictionary<ContainerInstance, FloatReference> _playersYPositionRatio;

        private FloatReference _averageSpeed;
        private FloatReference _firstPlayerSpeed;
        private FloatReference _lastPlayerSpeed;
        private FloatReference _standardDeviationSpeed;

        private FloatReference _currentLeadTimeCondition;
        private FloatReference _currentLeadSpeedCondition;
        private float _currentLeadTimeLeft;
        private ContainerInstance _currentContainerInstanceInLead;
        private float _timeReqProgressionTimer;
        private float _speedReqProgressionTimer;

        private IntReference _dampingMethodIndex;


        private bool _isGameInProgress = true;
        private RacingModeDebugInfo _racingModeDebugInfo;

        private enum SpeedReqCheckMethod
        {
            FromAverage,
            FromSecond,
            FromStart
        }

        private enum LeadReqProgressionMethod
        {
            Fixed,
            AnimCurve
        }

        private void Awake()
        {
            ScoreHandler.Instance.Initialize(_scoreHandlerData);
        }


        private void Start()
        {
            int numberOfActivePlayer = PlayerManager.Instance.GetNumberOfActivePlayer();

            // TODO: REMOVE THIS TEMP LINE (fake the player count)
            numberOfActivePlayer = useDebugSpawnContainer ? debugFakeNumberCount : numberOfActivePlayer;


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

                Initializer.SetCannonParameters(i, cannon, container, gameModeData,
                    ScoreHandler.Instance.GetPlayerScoreReference(i), gameModeData.skinData.playersSkinData[i], this);
                cannon.SetInputParameters(PlayerManager.Instance.GetPlayerInputHandler(i));
                cannon.SetCannonInputEnabled(true);
            }

            //// Racing Stuff!!!
            SetRacingModeParameters();
            SetContainerRacingParameters();

            SetRacingModeDebugInfoParameters();
        }

        private void Update()
        {
            if (!_isGameInProgress)
                return;

            // TODO: Clean this once we "hard set" the damping method
            _dampingMethodIndex.Variable.SetValue((int)_dampingMethod);

            UpdateSpeedParameters();
            UpdateRanking();

            // TODO: REMOVE THIS CONDITION
            if (checkLeadCondition)
                UpdateLead();
            CheckAndProcessWinCondition();
        }

        private void SetRacingModeDebugInfoParameters()
        {
            _racingModeDebugInfo = FindObjectOfType<RacingModeDebugInfo>();
            if (_racingModeDebugInfo == null)
                return;
            _racingModeDebugInfo.SetRacingModeParameters(_averageSpeed, _standardDeviationSpeed,
                _currentLeadTimeCondition, _currentLeadSpeedCondition);
            _racingModeDebugInfo.SetDebugActivationParameters(_isRacingModeDebugTextEnabled);
        }

        private void SetRacingModeParameters()
        {
            _averageSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _firstPlayerSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _lastPlayerSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _standardDeviationSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _currentLeadTimeCondition = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _currentLeadSpeedCondition = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _dampingMethodIndex = new IntReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };

            _playerCurrentSpeedReferences = new Dictionary<ContainerInstance, FloatReference>();
            _playerRankingReferences = new Dictionary<ContainerInstance, IntReference>();
            _playerLeadStatus = new Dictionary<ContainerInstance, BoolReference>();
            _playerLeadTimer = new Dictionary<ContainerInstance, FloatReference>();
            _playersYPositionRatio = new Dictionary<ContainerInstance, FloatReference>();

            foreach (var container in ContainerTracker.Instance.GetItems())
            {
                FloatReference newCurrentSpeedVar = new FloatReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
                IntReference newPlayerRankingRef = new IntReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
                BoolReference newPlayerLeadStatus = new BoolReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<BoolVariable>() };
                FloatReference newPlayerLeadTimer = new FloatReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
                var newYPositionRatioRef = new FloatReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };

                newPlayerLeadStatus.Variable.SetValue(false);

                _playerCurrentSpeedReferences[container] = newCurrentSpeedVar;
                _playerRankingReferences[container] = newPlayerRankingRef;
                _playerLeadStatus[container] = newPlayerLeadStatus;
                _playerLeadTimer[container] = newPlayerLeadTimer;
                _playersYPositionRatio[container] = newYPositionRatioRef;
            }

            _dampingMethodIndex.Variable.SetValue((int)_dampingMethod);

            _currentLeadTimeCondition.Variable.SetValue(_timeLeadConditionMinRange.x + _timeLeadConditionMinRange.y);
            _currentLeadSpeedCondition.Variable.SetValue(_speedLeadConditionMinRange.x + _speedLeadConditionMinRange.y);
        }

        private void SetContainerRacingParameters()
        {
            // TODO: clean up this, it's for a quick test
            var playerIndex = 1;
            foreach (var container in ContainerTracker.Instance.GetItems())
            {
                var containerRacing = container.GetComponent<ContainerRacingMode>();
                // containerRacing.SetAreaParameters(_containerMaxArea);
                containerRacing.SetSpeedParameters(_playerCurrentSpeedReferences[container], _averageSpeed,
                    _speedSoftCap, _firstPlayerSpeed, _lastPlayerSpeed);
                containerRacing.SetDampingParameters(_dampingMethodIndex, _dampingFixedPercent, _dampingFixedValue,
                    _dampingCurvePercent);
                containerRacing.SetComboParameters(_comboTimerFull, _acceleration);
                containerRacing.SetRankingParameters(_playerRankingReferences[container]);
                containerRacing.SetLeadParameters(_playerLeadStatus[container], _playerLeadTimer[container]);
                containerRacing.SetCollisionParameters(_ballImpactMultiplier);
                containerRacing.SetPositionParameters(_playersYPositionRatio[container], _minAdaptiveVerticalRange);
                containerRacing.SetDebugActivationParameters(_isContainerSpeedBarDebugEnabled,
                    _isContainerFullDebugTextEnabled, _isContainerAbridgedDebugTextEnabled, _debugScoreMultiplier);
                containerRacing.SetLayer($"Container{playerIndex}");
                playerIndex++;
            }
        }

        private void UpdateSpeedParameters()
        {
            // Average
            _averageSpeed.Variable.SetValue(_playerCurrentSpeedReferences.Sum(x => x.Value) /
                                            _playerCurrentSpeedReferences.Count);

            // Standard deviation
            _standardDeviationSpeed.Variable.SetValue(Mathf.Sqrt(
                _playerCurrentSpeedReferences.Sum(x => (x.Value - _averageSpeed) * (x.Value - _averageSpeed)) /
                _playerCurrentSpeedReferences.Count));
        }

        private void UpdateRanking()
        {
            var playerOrder = (from container in _playerCurrentSpeedReferences
                orderby container.Value.Value descending
                select container).ToList();

            _firstPlayerSpeed.Variable.SetValue(playerOrder.First().Value);
            _lastPlayerSpeed.Variable.SetValue(playerOrder.Last().Value);

            int rankingIndex = 1;
            for (int i = 0; i < playerOrder.Count(); i++)
            {
                if (i == 0)
                {
                    _playerRankingReferences[playerOrder[i].Key].Variable.SetValue(rankingIndex);
                    continue;
                }

                if (Mathf.Abs(playerOrder[i].Value.Value - playerOrder[i - 1].Value.Value) > Mathf.Epsilon)
                    rankingIndex += 1;

                _playerRankingReferences[playerOrder[i].Key].Variable.SetValue(rankingIndex);
            }
        }

        private void UpdateLead()
        {
            var currentFirst =
                _playerCurrentSpeedReferences.Aggregate((first, next) =>
                    next.Value > first.Value ? next : first);

            var passedPtsCheck = CheckPointsReqValue(currentFirst.Value);

            if (_alwaysReduceConditionValues)
                UpdateLeadRequirementsParameters();

            if (passedPtsCheck && currentFirst.Key == _currentContainerInstanceInLead)
            {
                _currentLeadTimeLeft -= Time.deltaTime;
                _playerLeadTimer[_currentContainerInstanceInLead].Variable.SetValue(_currentLeadTimeLeft);
                return;
            }

            if (_currentContainerInstanceInLead != null)
            {
                _playerLeadStatus[_currentContainerInstanceInLead].Variable.SetValue(false);
                _currentContainerInstanceInLead = null;
            }

            if (!passedPtsCheck)
            {
                if (!_alwaysReduceConditionValues)
                    UpdateLeadRequirementsParameters();
                return;
            }

            _currentContainerInstanceInLead = currentFirst.Key;
            _playerLeadStatus[_currentContainerInstanceInLead].Variable.SetValue(true);
            _currentLeadTimeLeft = _currentLeadTimeCondition;
            _playerLeadTimer[_currentContainerInstanceInLead].Variable.SetValue(_currentLeadTimeLeft);
        }

        private void UpdateLeadRequirementsParameters()
        {
            if (_leadReqProgressionMethod == LeadReqProgressionMethod.Fixed)
                return;
            _timeReqProgressionTimer += Time.deltaTime;
            _speedReqProgressionTimer += Time.deltaTime;

            _currentLeadTimeCondition.Variable.SetValue(
                _leadTimeReqCurve.Evaluate(Mathf.Clamp01(_timeReqProgressionTimer / _leadTimeConditionTimerRange)) *
                _timeLeadConditionMinRange.y + _timeLeadConditionMinRange.x);
            _currentLeadSpeedCondition.Variable.SetValue(
                _leadSpeedReqCurve.Evaluate(Mathf.Clamp01(_speedReqProgressionTimer / _leadSpeedConditionTimerRange)) *
                _speedLeadConditionMinRange.y + _speedLeadConditionMinRange.x);
        }

        private void CheckAndProcessWinCondition()
        {
            if (_currentContainerInstanceInLead == null || _playerLeadTimer[_currentContainerInstanceInLead] > 0f)
                return;

            foreach (var cannon in CannonTracker.Instance.GetItems())
                cannon.DisconnectCannonToPlayer();

            foreach (var container in ContainerTracker.Instance.GetItems())
            {
                if (container != _currentContainerInstanceInLead)
                    container.ContainerFailure();
                else
                    container.ContainerSuccess();
            }

            foreach (var ball in BallTracker.Instance.GetItems())
            {
                ball.SetSimulatedParameters(false);
            }
            _isGameInProgress = false;
        }

        private bool CheckPointsReqValue(float score)
        {
            return _speedReqCheckMethod switch
            {
                SpeedReqCheckMethod.FromAverage => score > _averageSpeed + _currentLeadSpeedCondition,
                SpeedReqCheckMethod.FromSecond => score > (_playerCurrentSpeedReferences.Count > 1
                    ? _playerCurrentSpeedReferences.OrderByDescending(r => r.Value.Value).Skip(1).FirstOrDefault()
                          .Value +
                      _currentLeadSpeedCondition
                    : _currentLeadSpeedCondition),
                SpeedReqCheckMethod.FromStart => score > _currentLeadSpeedCondition,
                _ => false
            };
        }

        // TODO: Move that behaviour in its own data type (it's not the job of the container to do that)
        public void OnBallFusion(BallInstance ballInstance)
        {
            var racingDebugInfo = ballInstance.ContainerInstance.GetComponent<ContainerRacingMode>();
            if (racingDebugInfo != null)
                racingDebugInfo.NewBallFused(ballInstance.ScoreValue);
        }
    }
}