using System;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    public abstract class ScoreHandlerDatattt : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public int PlayerIndex { get; private set; }

        public void SetParameters(int playerIndex)
        {
            PlayerIndex = playerIndex;
            SetParameters();
        }

        public abstract void SetActive(bool isActive);
        
        protected abstract void SetParameters();

        public abstract void UpdateScore();

        public abstract FloatReference GetScoreReference();

        public abstract void ResetScore();
    }
}