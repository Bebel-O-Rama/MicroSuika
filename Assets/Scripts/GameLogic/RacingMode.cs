using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.DebugInfo;
using MultiSuika.Utilities;
using MultiSuika.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.GameLogic
{
    public class RacingMode : MonoBehaviour, IGameMode
    {
        [Header("Speed Parameters")]
        [SerializeField] private FloatReference _speedSoftCap; // 1000

        [Header("Ball Collision Parameters")] 
        [SerializeField] private FloatReference _ballImpactMultiplier; // 2 
        
        [Header("Damping Parameters")]
        [SerializeField] private ContainerRacingMode.DampingMethod _dampingMethod; // AnimCurve
        [SerializeField] private FloatReference _dampingFixedPercent; // 0.02
        [SerializeField] private FloatReference _dampingFixedValue; // 1
        [Tooltip("The animation curve can't be modified at runtime")]
        [SerializeField] private AnimationCurve _dampingCurvePercent; // (0.0;0.0), (0.5;0.015), (1.0;0.05)
        
        [Header("Area Parameters")]
        [SerializeField] private FloatReference _containerMaxArea; // 60

        [Header("Combo Parameters")]
        [SerializeField] private FloatReference _comboTimerFull; // 3
        [SerializeField] private FloatReference _acceleration; // 3

        [Header("Lead Parameters")]
        [Tooltip(
            "If false, only increase the timer reducing the value of the condition variables when nobody is winning. Otherwise, always increase the timer")]
        [SerializeField] private bool _alwaysReduceConditionValues;
        [SerializeField] private LeadReqProgressionMethod _leadReqProgressionMethod;
        [SerializeField] private SpeedReqCheckMethod _speedReqCheckMethod;
        [SerializeField] [Min(0f)] private Vector2 _timeLeadConditionMinRange;
        [SerializeField] [Min(0f)] private Vector2 _speedLeadConditionMinRange;
        [SerializeField] [Min(1f)] private float _leadTimeConditionTimerRange;
        [SerializeField] [Min(1f)] private float _leadSpeedConditionTimerRange;
        [SerializeField] private AnimationCurve _leadTimeReqCurve;
        [SerializeField] private AnimationCurve _leadSpeedReqCurve;

        [Header("Movement Parameters")] 
        [SerializeField] private FloatReference _minAdaptiveVerticalRange;

        [Header("GameMode Parameters")] 
        [SerializeField] public GameData gameData;
        [SerializeField] public GameModeData gameModeData;

        [Header("DEBUG parameters")] 
        public bool useDebugSpawnContainer = false;
        [Range(0, 4)] [SerializeField] public int debugFakeNumberCount = 2;
        [SerializeField] public bool checkLeadCondition = true;
        [SerializeField] private BoolReference _isRacingModeDebugTextEnabled;
        [SerializeField] private BoolReference _isContainerSpeedBarDebugEnabled;
        [SerializeField] private BoolReference _isContainerFullDebugTextEnabled;
        [SerializeField] private BoolReference _isContainerAbridgedDebugTextEnabled;
        [SerializeField] private FloatReference _debugScoreMultiplier;

        private int _numberPlayerConnected;
        private List<PlayerInputHandler> _playerInputHandlers;
        private GameObject _versusGameInstance;
        private List<Container.Container> _containers;
        private List<Cannon.Cannon> _cannons;
        private BallTracker _ballTracker = new BallTracker();

        private Dictionary<Container.Container, FloatReference> _playerCurrentSpeedReferences;
        private Dictionary<Container.Container, IntReference> _playerRankingReferences;
        private Dictionary<Container.Container, BoolReference> _playerLeadStatus;
        private Dictionary<Container.Container, FloatReference> _playerLeadTimer;
        private Dictionary<Container.Container, FloatReference> _playersYPositionRatio;

        private FloatReference _averageSpeed;
        private FloatReference _firstPlayerSpeed;
        private FloatReference _lastPlayerSpeed;
        private FloatReference _standardDeviationSpeed;
        private FloatReference _currentLeadTimeCondition;
        private FloatReference _currentLeadSpeedCondition;
        private IntReference _dampingMethodIndex;

        private Container.Container _currentContainerInLead;
        private float _currentLeadTimeLeft;
        private float _timeReqProgressionTimer;
        private float _speedReqProgressionTimer;

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
            _numberPlayerConnected = gameData.GetConnectedPlayerQuantity();

            // TODO: REMOVE THIS TEMP LINE (fake the player count)
            _numberPlayerConnected = useDebugSpawnContainer ? debugFakeNumberCount : _numberPlayerConnected;

            //// Init and set containers
            _containers =
                Initializer.InstantiateContainers(_numberPlayerConnected, gameModeData);
            Initializer.SetContainersParameters(_containers, gameModeData);

            //// Init and set cannons
            _cannons = Initializer.InstantiateCannons(_numberPlayerConnected, gameModeData,
                _containers);
            Initializer.SetCannonsParameters(_cannons, _containers, _ballTracker, gameModeData,
                gameData.playerDataList, this);

            //// Init and set playerInputHandlers
            _playerInputHandlers =
                Initializer.InstantiatePlayerInputHandlers(gameData.GetConnectedPlayersData(), gameModeData);
            Initializer.ConnectCannonsToPlayerInputs(_cannons, _playerInputHandlers);
            
            //// Racing Stuff!!!
            SetRacingModeParameters();
            SetContainerRacingParameters();
        }

        private void Start()
        {
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

            _playerCurrentSpeedReferences = new Dictionary<Container.Container, FloatReference>();
            _playerRankingReferences = new Dictionary<Container.Container, IntReference>();
            _playerLeadStatus = new Dictionary<Container.Container, BoolReference>();
            _playerLeadTimer = new Dictionary<Container.Container, FloatReference>();
            _playersYPositionRatio = new Dictionary<Container.Container, FloatReference>();

            foreach (var container in _containers)
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
            foreach (var container in _containers)
            {
                var containerRacing = container.GetComponent<ContainerRacingMode>();
                containerRacing.SetAreaParameters(_ballTracker.GetBallAreaForContainer(container), _containerMaxArea);
                containerRacing.SetSpeedParameters(_playerCurrentSpeedReferences[container], _averageSpeed, _speedSoftCap, _firstPlayerSpeed, _lastPlayerSpeed);
                containerRacing.SetDampingParameters(_dampingMethodIndex, _dampingFixedPercent, _dampingFixedValue,
                    _dampingCurvePercent);
                containerRacing.SetComboParameters(_comboTimerFull, _acceleration);
                containerRacing.SetRankingParameters(_playerRankingReferences[container]);
                containerRacing.SetLeadParameters(_playerLeadStatus[container], _playerLeadTimer[container]);
                containerRacing.SetCollisionParameters(_ballImpactMultiplier);
                containerRacing.SetPositionParameters(_playersYPositionRatio[container], _minAdaptiveVerticalRange);
                containerRacing.SetDebugActivationParameters(_isContainerSpeedBarDebugEnabled, _isContainerFullDebugTextEnabled, _isContainerAbridgedDebugTextEnabled, _debugScoreMultiplier);
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

            if (passedPtsCheck && currentFirst.Key == _currentContainerInLead)
            {
                _currentLeadTimeLeft -= Time.deltaTime;
                _playerLeadTimer[_currentContainerInLead].Variable.SetValue(_currentLeadTimeLeft);
                return;
            }

            if (_currentContainerInLead != null)
            {
                _playerLeadStatus[_currentContainerInLead].Variable.SetValue(false);
                _currentContainerInLead = null;
            }

            if (!passedPtsCheck)
            {
                if (!_alwaysReduceConditionValues)
                    UpdateLeadRequirementsParameters();
                return;
            }

            _currentContainerInLead = currentFirst.Key;
            _playerLeadStatus[_currentContainerInLead].Variable.SetValue(true);
            _currentLeadTimeLeft = _currentLeadTimeCondition;
            _playerLeadTimer[_currentContainerInLead].Variable.SetValue(_currentLeadTimeLeft);
        }

        private void UpdateLeadRequirementsParameters()
        {
            if (_leadReqProgressionMethod == LeadReqProgressionMethod.Fixed)
                return;
            _timeReqProgressionTimer += Time.deltaTime;
            _speedReqProgressionTimer += Time.deltaTime;

            _currentLeadTimeCondition.Variable.SetValue(_leadTimeReqCurve.Evaluate(Mathf.Clamp01(_timeReqProgressionTimer / _leadTimeConditionTimerRange)) *
                _timeLeadConditionMinRange.y + _timeLeadConditionMinRange.x);
            _currentLeadSpeedCondition.Variable.SetValue(_leadSpeedReqCurve.Evaluate(Mathf.Clamp01(_speedReqProgressionTimer / _leadSpeedConditionTimerRange)) *
                _speedLeadConditionMinRange.y + _speedLeadConditionMinRange.x);
        }

        private void CheckAndProcessWinCondition()
        {
            if (_currentContainerInLead == null || _playerLeadTimer[_currentContainerInLead] > 0f)
                return;
            
            foreach (var cannon in _cannons)
                cannon.DisconnectCannonToPlayer();
            
            foreach (var container in _containers)
            {
                if (container != _currentContainerInLead)
                    container.ContainerFailure();
                else
                    container.ContainerSuccess();

                foreach (var ball in _ballTracker.GetBallsForContainer(container))
                    ball.SetBallFreeze(true);
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

        public void OnBallFusion(Ball.Ball ball)
        {
            var racingDebugInfo = ball.container.GetComponent<ContainerRacingMode>();
            if (racingDebugInfo != null)
                racingDebugInfo.NewBallFused(ball.scoreValue);
        }
    }
}