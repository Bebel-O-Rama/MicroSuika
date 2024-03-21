using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    public interface IScoreModifierData
    {
        public ScoreModifierStatus ApplyModifier();

        public void SetActive(bool isActive);
    }

    public enum ScoreModifierStatus
    {
        Continue,
        Stop
        
    }
}
