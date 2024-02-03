using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Ball
{
    public class BallTracker
    {
        private Dictionary<Container.Container, List<Ball>> _ballsPerContainer = new Dictionary<Container.Container, List<Ball>>();
        
        public List<Ball> GetBallsForContainer(Container.Container container) =>
            _ballsPerContainer.ContainsKey(container) ? _ballsPerContainer[container] : new List<Ball>();
        
        public void RegisterBall(Ball ball, Container.Container container)
        {
            if (!_ballsPerContainer.ContainsKey(container))
                _ballsPerContainer[container] = new List<Ball>();
            if (!_ballsPerContainer[container].Contains(ball))
                _ballsPerContainer[container].Add(ball);
        }

        public void UnregisterBall(Ball ball, Container.Container container)
        {
            if (!_ballsPerContainer.ContainsKey(container))
                return;
            if (_ballsPerContainer[container].Contains(ball))
                _ballsPerContainer[container].Remove(ball);
        }
    }
}