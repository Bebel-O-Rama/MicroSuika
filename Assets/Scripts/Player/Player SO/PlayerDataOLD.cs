using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Player
{
    [CreateAssetMenu(menuName = "Player/Player Game Data (OLD)")]
    public class PlayerDataOLD : ScriptableObject
    {
        [Header("Player number")]
        [Range(0, 3)] public int playerIndexNumber;

        [Header("Score Reference")]
        public IntReference mainScore;
        
        public void ResetMainScore() => mainScore.Variable.SetValue(0);
    }
}
