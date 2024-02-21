using System.Collections.Generic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Ball
{
    public class BallTracker
    {
        private Dictionary<Container.Container, List<Ball>> _ballsPerContainer = new Dictionary<Container.Container, List<Ball>>();
        private Dictionary<Container.Container, FloatReference> _ballAreaPerContainer = new Dictionary<Container.Container, FloatReference>();

        public List<Ball> GetBallsForContainer(Container.Container container) =>
            _ballsPerContainer.ContainsKey(container) 
                ? _ballsPerContainer[container] 
                : new List<Ball>();

        public FloatReference GetBallAreaForContainer(Container.Container container) =>
            _ballAreaPerContainer.ContainsKey(container)
                ? _ballAreaPerContainer[container]
                : CreateNewBallAreaReference(container);
        
        public void RegisterBall(Ball ball, Container.Container container)
        {
            CheckForInitialSetup(container);
            if (_ballsPerContainer[container].Contains(ball))
                return;    
            _ballsPerContainer[container].Add(ball);
            _ballAreaPerContainer[container].Variable.ApplyChange(ball.GetBallArea());
        }

        public void UnregisterBall(Ball ball, Container.Container container)
        {
            if (!_ballsPerContainer.ContainsKey(container) || !_ballAreaPerContainer.ContainsKey(container))
                return;
            if (!_ballsPerContainer[container].Contains(ball))
                return;
            _ballsPerContainer[container].Remove(ball);
            _ballAreaPerContainer[container].Variable.ApplyChange(-ball.GetBallArea());
        }

        private void CheckForInitialSetup(Container.Container container)
        {
            if (!_ballsPerContainer.ContainsKey(container))
                _ballsPerContainer[container] = new List<Ball>();
            if (!_ballAreaPerContainer.ContainsKey(container))
                CreateNewBallAreaReference(container);
        }
        
        private FloatReference CreateNewBallAreaReference(Container.Container container)
        {
            FloatReference newAreaRef = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _ballAreaPerContainer[container] = newAreaRef;
            return newAreaRef;
        }
    }
}