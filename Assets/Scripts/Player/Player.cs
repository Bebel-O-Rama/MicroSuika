using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Player
{
    public class Player : MonoBehaviour
    {
        public Cannon.Cannon cannon;
        public IntReference score;
        public int playerIndex;
        [SerializeField] 
        public PlayerInputHandler playerInputHandler;
    }
}
