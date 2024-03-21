using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.ScoreSystemTransition;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Manager
{
    public class ScoreHandler
    {
        private List<ScoreHandlerData> _scoreHandlers;

        #region Singleton

        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static ScoreHandler Instance => _instance ??= new ScoreHandler();

        private static ScoreHandler _instance;

        private ScoreHandler()
        {
        }

        #endregion
        
        // For now, let's hardset that to 4
        public void Initialize(ScoreHandlerData scoreHandlerData)
        {
            _scoreHandlers = new List<ScoreHandlerData>();
            for (int i = 0; i < 4; i++)
            {
                var scoreHandler = Object.Instantiate(scoreHandlerData);
                scoreHandler.SetParameters(i);
                _scoreHandlers.Add(scoreHandler);
            }
        }

        public void SetActive(bool isActive)
        {
            foreach (var scoreHandler in _scoreHandlers)
            {
                scoreHandler.SetActive(true);
            }
        }

        public void UpdateScore()
        {
            foreach (var scoreHandler in _scoreHandlers)
            {
                scoreHandler.UpdateScore();
            }
        }

        public FloatReference GetPlayerScoreReference(int playerIndex) => _scoreHandlers[playerIndex].GetScoreReference();
        
        // Find a better way to generalize all that
        public List<FloatReference> GetPlayerScoreReferences() => _scoreHandlers.Select(s => s.GetScoreReference()).ToList();
        public void ResetScoreInformation() => _scoreHandlers.ForEach(s => s.ResetScore());
    }
}
