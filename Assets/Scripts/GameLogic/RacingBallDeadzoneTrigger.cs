using System.Collections.Generic;
using System.Linq;
using MultiSuika.Container;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class RacingBallDeadzoneTrigger : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            var ball = other.GetComponent<Ball.Ball>();
            ball.container.GetComponent<ContainerRacingMode>().DamageReceived(ball);
        }
    }
}