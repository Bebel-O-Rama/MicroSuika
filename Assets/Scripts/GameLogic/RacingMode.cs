using System;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using MultiSuika.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.GameLogic
{
    public class RacingMode : MonoBehaviour, IGameMode
    {
        [Header("Testing Parameters")]
        // [SerializeField] public bool canShareRanking;
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

        [Header("Visual Debug Parameters")] 
        [SerializeField] public TMP_Text _tmpMean;

        [SerializeField] public TMP_Text _tmpSD;
        [SerializeField] public TMP_Text _tmpTimeReq;
        [SerializeField] public TMP_Text _tmpPointsReq;

        [SerializeField] public GameData gameData;
        [SerializeField] public GameModeData gameModeData;

        private int _numberPlayerConnected;
        private List<PlayerInputHandler> _playerInputHandlers;
        private GameObject _versusGameInstance;
        private List<Container.Container> _containers;
        private List<Cannon.Cannon> _cannons;
        private BallTracker _ballTracker = new BallTracker();

        private Dictionary<Container.Container, FloatReference> _playerCurrentSpeedReferences;
        private Dictionary<Container.Container, FloatReference> _playerLeadTimer;
        private Dictionary<Container.Container, IntReference> _playerRankingReferences;
        private FloatReference _averageSpeed;

        private FloatReference _standardDeviationSpeed;
        
        private Container.Container _currentContainerInLead;
        private float _currentLeadTimeCondition;
        private float _currentLeadSpeedCondition;
        private float _currentLeadTimeLeft;
        private float _timeReqProgressionTimer;
        private float _speedReqProgressionTimer;

        private bool _isGameInProgress = true;
        
        [Header("DEBUG DEBUG DEBUG")] public bool useDebugSpawnContainer = false;
        [Range(0, 4)] [SerializeField] public int debugFakeNumberCount = 2;

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
            _versusGameInstance = new GameObject("Versus Game Instance");

            _containers =
                Initializer.InstantiateContainers(_numberPlayerConnected, gameModeData, _versusGameInstance.transform);
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
            SetupRacingDataUI();
        }

        private void Update()
        {
            if (!_isGameInProgress)
                return;
            UpdateSpeedParameters();
            UpdateRanking();
            UpdateLead();
            UpdateDebugInfo();
            CheckAndProcessWinCondition();
        }

        private void SetupRacingDataUI()
        {
            _averageSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _standardDeviationSpeed = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };

            _playerCurrentSpeedReferences = new Dictionary<Container.Container, FloatReference>();
            _playerRankingReferences = new Dictionary<Container.Container, IntReference>();
            _playerLeadTimer = new Dictionary<Container.Container, FloatReference>();

            foreach (var container in _containers)
            {
                FloatReference newCurrentSpeedVar = new FloatReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
                IntReference newPlayerRankingRef = new IntReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<IntVariable>() };
                FloatReference newPlayerLeadTimer = new FloatReference
                    { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };

                _playerCurrentSpeedReferences[container] = newCurrentSpeedVar;
                _playerRankingReferences[container] = newPlayerRankingRef;
                _playerLeadTimer[container] = newPlayerLeadTimer;

                var newRacingDebugInfo = container.GetComponent<RacingDebugInfo>();
                newRacingDebugInfo.ballAreaRef = _ballTracker.GetBallAreaForContainer(container);
                newRacingDebugInfo.currentSpeed = newCurrentSpeedVar;
                newRacingDebugInfo.averageSpeed = _averageSpeed;
                newRacingDebugInfo.currentRanking = newPlayerRankingRef;
                newRacingDebugInfo.leadTimer = newPlayerLeadTimer;
            }

            _currentLeadTimeCondition = _timeLeadConditionMinRange.x + _timeLeadConditionMinRange.y;
            _currentLeadSpeedCondition = _speedLeadConditionMinRange.x + _speedLeadConditionMinRange.y;
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
                _currentContainerInLead.GetComponent<RacingDebugInfo>().SetLeadStatus(false);

            if (!passedPtsCheck)
            {
                if (!_alwaysReduceConditionValues)
                    UpdateLeadRequirementsParameters();
                return;
            }

            _currentContainerInLead = currentFirst.Key;
            _currentLeadTimeLeft = _currentLeadTimeCondition;
            _playerLeadTimer[_currentContainerInLead].Variable.SetValue(_currentLeadTimeLeft);
            _currentContainerInLead.GetComponent<RacingDebugInfo>().SetLeadStatus(true);
        }

        private void UpdateDebugInfo()
        {
            _tmpMean.text = string.Format($"{_averageSpeed.Value:0.00}");
            _tmpSD.text = string.Format($"{_standardDeviationSpeed.Value:0.00}");
            _tmpTimeReq.text = string.Format($"{_currentLeadTimeCondition:0.00}");
            _tmpPointsReq.text = string.Format($"{_currentLeadSpeedCondition:0}");
        }

        private void UpdateLeadRequirementsParameters()
        {
            if (_leadReqProgressionMethod == LeadReqProgressionMethod.Fixed)
                return;
            _timeReqProgressionTimer += Time.deltaTime;
            _speedReqProgressionTimer += Time.deltaTime;

            _currentLeadTimeCondition =
                _leadTimeReqCurve.Evaluate(Mathf.Clamp01(_timeReqProgressionTimer / _leadTimeConditionTimerRange)) *
                _timeLeadConditionMinRange.y + _timeLeadConditionMinRange.x;
            _currentLeadSpeedCondition =
                _leadSpeedReqCurve.Evaluate(Mathf.Clamp01(_speedReqProgressionTimer / _leadSpeedConditionTimerRange)) *
                _speedLeadConditionMinRange.y + _speedLeadConditionMinRange.x;
        }

        private void CheckAndProcessWinCondition()
        {
            if (_currentContainerInLead == null || _playerLeadTimer[_currentContainerInLead] > 0f)
                return;

            foreach (var cannon in _cannons)
                cannon.DisconnectCannonToPlayer();
            foreach (var container in _containers.Where(container => container != _currentContainerInLead))
            {
                container.GetComponent<RacingDebugInfo>().SetDebugUpdates(false);
                if (container != _currentContainerInLead)
                    container.ContainerFailure();
                else
                    container.ContainerSuccess();
            }

            _isGameInProgress = false;
        }
        

        private bool CheckPointsReqValue(float score)
        {
            return _speedReqCheckMethod switch
            {
                SpeedReqCheckMethod.FromAverage => score > _averageSpeed + _currentLeadSpeedCondition,
                SpeedReqCheckMethod.FromSecond => score > (_playerCurrentSpeedReferences.Count > 1
                    ? _playerCurrentSpeedReferences.OrderByDescending(r => r.Value.Value).Skip(1).FirstOrDefault().Value +
                      _currentLeadSpeedCondition
                    : _currentLeadSpeedCondition),
                SpeedReqCheckMethod.FromStart => score > _currentLeadSpeedCondition,
                _ => false
            };
        }

        public void OnBallFusion(Ball.Ball ball)
        {
            var racingDebugInfo =ball.container.GetComponent<RacingDebugInfo>();
            if (racingDebugInfo != null)
                racingDebugInfo.NewBallFused(ball.scoreValue);
        }
    }
}