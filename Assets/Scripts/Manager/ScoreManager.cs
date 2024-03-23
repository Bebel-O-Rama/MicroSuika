using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Container;
using MultiSuika.GameLogic;
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
        }

        #endregion

        [SerializeField] private List<ScoreHandler> _scoreHandlers;
        [SerializeField] private ScoreHandlerData _scoreHandlerData;

        private void Start()
        {
            foreach (var scoreHandler in _scoreHandlers)
            {
                scoreHandler.SetScoreHandlerData(_scoreHandlerData);
            }
        }

        public FloatReference GetCurrentSpeedReference(int playerIndex) =>
            _scoreHandlers[playerIndex].GetCurrentSpeedReference();
        public FloatReference GetTargetSpeedReference(int playerIndex) =>
            _scoreHandlers[playerIndex].GetTargetSpeedReference();
        // public IntReference GetComboReference(int playerIndex) =>
        //     _scoreHandlers[playerIndex].GetComboReference();

        public void RemoveScoreHandler(int playerIndex)
        {
            _scoreHandlers.RemoveAt(playerIndex);
        }

        public void ResetScoreInformation() => _scoreHandlers.ForEach(s => s.ResetScore());

        public ActionMethodPlayerWrapper<(int, float)> OnComboIncrement = new ActionMethodPlayerWrapper<(int, float)>();
        public ActionMethodPlayerWrapper<int> OnComboLost = new ActionMethodPlayerWrapper<int>();
    }
}