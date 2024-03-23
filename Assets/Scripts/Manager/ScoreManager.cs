using System.Collections.Generic;
using System.Linq;
using MultiSuika.Container;
using MultiSuika.ScoreSystemTransition;
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
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            Initialize();
        }

        #endregion

        // [SerializeField] private ScoreHandlerData _scoreHandlerData;
        [SerializeField] private List<ScoreHandler> _scoreHandlers;
        [SerializeField] private float _minAdaptiveVerticalRange; // 500

        private FloatReference _averageSpeed;
        private readonly Dictionary<int, FloatReference> _currentSpeedRefs = new Dictionary<int, FloatReference>();
        private readonly Dictionary<int, FloatReference> _normalizedSpeedRefs = new Dictionary<int, FloatReference>();
        private readonly List<int> _playerRankings = new List<int>();

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
                // scoreHandler.SetScoreHandlerData(_scoreHandlerData);
                _currentSpeedRefs[scoreHandler.playerIndex] = scoreHandler.GetCurrentSpeedReference();
                _normalizedSpeedRefs[scoreHandler.playerIndex] = new FloatReference();
                _playerRankings.Add(scoreHandler.playerIndex);
            }
        }

        private void Update()
        {
            UpdateRanking();
            UpdateNormalizedSpeed();
            UpdateAverageSpeed();
        }

        private void UpdateRanking()
        {
            var sortedPlayers = new List<KeyValuePair<int, FloatReference>>(_currentSpeedRefs);
            sortedPlayers.Sort((a, b) => b.Value.Value.CompareTo(a.Value));

            _playerRankings.Clear();
            foreach (var player in sortedPlayers)
            {
                _playerRankings.Add(player.Key);
            }
        }

        private void UpdateNormalizedSpeed()
        {
            float lowestSpeed = _currentSpeedRefs[_playerRankings.Last()];
            float highestSpeed = _currentSpeedRefs[_playerRankings.First()];
            float currentRange = Mathf.Max(_minAdaptiveVerticalRange,
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

        public void ClearPlayerScoreComponents(int playerIndex)
        {
            OnComboIncrement.Clear(playerIndex);
            OnComboLost.Clear(playerIndex);

            _scoreHandlers.RemoveAt(playerIndex);
            _currentSpeedRefs.Remove(playerIndex);
            _normalizedSpeedRefs.Remove(playerIndex);
            _playerRankings.Remove(playerIndex);
        }

        public void ResetScoreInformation() => _scoreHandlers.ForEach(s => s.ResetScore());

        public FloatReference GetCurrentSpeedReference(int playerIndex) =>
            _scoreHandlers[playerIndex].GetCurrentSpeedReference();

        public FloatReference GetTargetSpeedReference(int playerIndex) =>
            _scoreHandlers[playerIndex].GetTargetSpeedReference();

        public FloatReference GetNormalizedSpeedReference(int playerIndex) => _normalizedSpeedRefs[playerIndex];

        public List<int> GetPlayerRankings() => _playerRankings;
        public int GetPlayerRanking(int playerIndex) => _playerRankings.FindIndex(r => r == playerIndex);

        public FloatReference GetAverageSpeedReference() => _averageSpeed;

        public float GetSpeedSoftCap() => _scoreHandlers[0].GetSpeedSoftCap();
    }
}