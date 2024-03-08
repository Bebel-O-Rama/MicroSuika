using System.Collections.Generic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Ball
{
    public class BallTracker
    {
        private Dictionary<Container.Container, List<BallInstance>> _ballsPerContainer = new Dictionary<Container.Container, List<BallInstance>>();
        private Dictionary<Container.Container, FloatReference> _ballAreaPerContainer = new Dictionary<Container.Container, FloatReference>();

        public List<BallInstance> GetBallsForContainer(Container.Container container) =>
            _ballsPerContainer.ContainsKey(container) 
                ? _ballsPerContainer[container] 
                : new List<BallInstance>();

        public FloatReference GetBallAreaForContainer(Container.Container container) =>
            _ballAreaPerContainer.ContainsKey(container)
                ? _ballAreaPerContainer[container]
                : CreateNewBallAreaReference(container);
        
        public void RegisterBall(BallInstance ballInstance, Container.Container container)
        {
            CheckForInitialSetup(container);
            if (_ballsPerContainer[container].Contains(ballInstance))
                return;    
            _ballsPerContainer[container].Add(ballInstance);
            _ballAreaPerContainer[container].Variable.ApplyChange(ballInstance.GetBallArea());
        }

        public void UnregisterBall(BallInstance ballInstance, Container.Container container)
        {
            if (!_ballsPerContainer.ContainsKey(container) || !_ballAreaPerContainer.ContainsKey(container))
                return;
            if (!_ballsPerContainer[container].Contains(ballInstance))
                return;
            _ballsPerContainer[container].Remove(ballInstance);
            _ballAreaPerContainer[container].Variable.ApplyChange(-ballInstance.GetBallArea());
        }

        private void CheckForInitialSetup(Container.Container container)
        {
            if (!_ballsPerContainer.ContainsKey(container))
                _ballsPerContainer[container] = new List<BallInstance>();
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