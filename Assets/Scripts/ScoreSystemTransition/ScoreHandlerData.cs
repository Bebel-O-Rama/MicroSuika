using System;
using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    public abstract class ScoreHandlerData : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public int PlayerIndex { get; private set; }

        public void Init(int playerIndex)
        {
            PlayerIndex = playerIndex;
            Init();
        }
        
        protected abstract void Init();



        public abstract void UpdateScore();

        public abstract FloatReference GetScoreReference();

        public abstract void ResetScore();
    }
}