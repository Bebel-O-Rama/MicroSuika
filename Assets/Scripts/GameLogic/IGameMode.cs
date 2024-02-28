using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public interface IGameMode
    {
        public void OnBallFusion(Ball.Ball ball);
    }
}
