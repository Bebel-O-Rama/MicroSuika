using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public interface IGameModeManager
    {
        public void OnBallFusion(Ball.BallInstance ballInstance);
    }
}
