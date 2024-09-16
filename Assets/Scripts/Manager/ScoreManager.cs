using System.Collections.Generic;
using System.Linq;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        #region Singleton

        public static ScoreManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);

            Initialize();
        }

        #endregion

        [SerializeField] [Min(1)] private int _finalScoreTarget;
        [SerializeField] private List<ScoreHandler> _scoreHandlers;
        [SerializeField] private float _minAdaptiveVerticalRange; // 500

        private FloatReference _averageSpeed;
        private readonly Dictionary<int, FloatReference> _currentSpeedRefs = new Dictionary<int, FloatReference>();
        private readonly Dictionary<int, FloatReference> _normalizedSpeedRefs = new Dictionary<int, FloatReference>();
        private readonly List<int> _playerSpeedRankings = new List<int>();

        public ActionMethodPlayerWrapper<(int, float)> OnComboIncrement = new ActionMethodPlayerWrapper<(int, float)>();
        public ActionMethodPlayerWrapper<int> OnComboLost = new ActionMethodPlayerWrapper<int>();

        private void Initialize()
        {
            _averageSpeed = new FloatReference();
        }

        private void Start()
        {
            foreach (var scoreHandler in _scoreHandlers)
            {
                _currentSpeedRefs[scoreHandler.PlayerIndex] = scoreHandler.CurrentSpeed;
                _normalizedSpeedRefs[scoreHandler.PlayerIndex] = new FloatReference();
                _playerSpeedRankings.Add(scoreHandler.PlayerIndex);
            }
        }

        private void Update()
        {
            UpdateSpeedRanking();
            UpdateNormalizedSpeed();
            UpdateAverageSpeed();
        }

        #region SpeedManagement
        
        private void UpdateSpeedRanking()
        {
            var sortedPlayers = new List<KeyValuePair<int, FloatReference>>(_currentSpeedRefs);
            sortedPlayers.Sort((a, b) => b.Value.Value.CompareTo(a.Value));

            _playerSpeedRankings.Clear();
            foreach (var player in sortedPlayers)
            {
                _playerSpeedRankings.Add(player.Key);
            }
        }

        private void UpdateNormalizedSpeed()
        {
            float lowestSpeed = _currentSpeedRefs[_playerSpeedRankings.Last()];
            float highestSpeed = _currentSpeedRefs[_playerSpeedRankings.First()];
            var currentRange = Mathf.Max(_minAdaptiveVerticalRange,
                highestSpeed - lowestSpeed);

            foreach (var player in _normalizedSpeedRefs)
            {
                var normalizedSpeed = Mathf.Clamp01((_currentSpeedRefs[player.Key] - lowestSpeed) / currentRange);
                player.Value.Variable.SetValue(normalizedSpeed);
            }
        }

        private void UpdateAverageSpeed()
        {
            _averageSpeed.Variable.SetValue(_currentSpeedRefs.Sum(x => x.Value) / _currentSpeedRefs.Count);
        }
        
        #endregion
        
        #region PlayerScore
        
        public void ClearPlayerScoreComponents(int playerIndex)
        {
            OnComboIncrement.Clear(playerIndex);
            OnComboLost.Clear(playerIndex);

            _scoreHandlers.RemoveAt(playerIndex);
            _currentSpeedRefs.Remove(playerIndex);
            _normalizedSpeedRefs.Remove(playerIndex);
            _playerSpeedRankings.Remove(playerIndex);
        }

        public bool UpdateWinnerScore(int playerIndex)
        {
            var newScore = _scoreHandlers[playerIndex].IncrementScore();
            return newScore >= _finalScoreTarget;
        }

        public void ResetScores()
        {
            foreach (var handler in _scoreHandlers)
            {
                handler.ResetScore();
            }
        }
        
        #endregion
        
        #region Getter/Setter
        
        public void ResetSpeedInformation() => _scoreHandlers.ForEach(s => s.ResetSpeedHandler());

        public FloatReference GetCurrentSpeedReference(int playerIndex) =>
            _scoreHandlers[playerIndex].CurrentSpeed;

        public FloatReference GetTargetSpeedReference(int playerIndex) =>
            _scoreHandlers[playerIndex].TargetSpeed;

        public FloatReference GetNormalizedSpeedReference(int playerIndex) => _normalizedSpeedRefs[playerIndex];

        public List<int> GetPlayerSpeedRankings() => _playerSpeedRankings;
        public int GetPlayerSpeedRanking(int playerIndex) => _playerSpeedRankings.FindIndex(r => r == playerIndex);

        public FloatReference GetAverageSpeedReference() => _averageSpeed;

        public List<int> GetPlayerScores() =>
            _scoreHandlers
                .Select(handler => handler.PlayerScore)
                .ToList();
        
        #endregion

    }
}