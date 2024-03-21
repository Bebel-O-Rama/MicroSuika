using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.ScoreSystemTransition
{
    public interface IScoreModifierData<in TArgs>
    {
        public void SetParameters(int playerIndex, TArgs args);
        public void SetActive(bool isActive);
        public ScoreModifierStatus ApplyModifier();
    }

    public enum ScoreModifierStatus
    {
        Continue,
        Stop
    }
}
