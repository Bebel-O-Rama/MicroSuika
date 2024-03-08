using System.Collections;
using System.Collections.Generic;
using MultiSuika.Ball;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public interface IGameModeManager
    {
        public void OnBallFusion(BallInstance ballInstance);
    }
}
