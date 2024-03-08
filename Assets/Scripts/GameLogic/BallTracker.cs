using System.Collections.Generic;
using MultiSuika.Ball;
using MultiSuika.Container;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class BallTracker
    {
        private Dictionary<ContainerInstance, List<BallInstance>> _ballsPerContainer = new Dictionary<ContainerInstance, List<BallInstance>>();
        private Dictionary<ContainerInstance, FloatReference> _ballAreaPerContainer = new Dictionary<ContainerInstance, FloatReference>();

        public List<BallInstance> GetBallsForContainer(ContainerInstance containerInstance) =>
            _ballsPerContainer.ContainsKey(containerInstance) 
                ? _ballsPerContainer[containerInstance] 
                : new List<BallInstance>();

        public FloatReference GetBallAreaForContainer(ContainerInstance containerInstance) =>
            _ballAreaPerContainer.ContainsKey(containerInstance)
                ? _ballAreaPerContainer[containerInstance]
                : CreateNewBallAreaReference(containerInstance);
        
        public void RegisterBall(BallInstance ballInstance, ContainerInstance containerInstance)
        {
            CheckForInitialSetup(containerInstance);
            if (_ballsPerContainer[containerInstance].Contains(ballInstance))
                return;    
            _ballsPerContainer[containerInstance].Add(ballInstance);
            _ballAreaPerContainer[containerInstance].Variable.ApplyChange(ballInstance.GetBallArea());
        }

        public void UnregisterBall(BallInstance ballInstance, ContainerInstance containerInstance)
        {
            if (!_ballsPerContainer.ContainsKey(containerInstance) || !_ballAreaPerContainer.ContainsKey(containerInstance))
                return;
            if (!_ballsPerContainer[containerInstance].Contains(ballInstance))
                return;
            _ballsPerContainer[containerInstance].Remove(ballInstance);
            _ballAreaPerContainer[containerInstance].Variable.ApplyChange(-ballInstance.GetBallArea());
        }

        private void CheckForInitialSetup(ContainerInstance containerInstance)
        {
            if (!_ballsPerContainer.ContainsKey(containerInstance))
                _ballsPerContainer[containerInstance] = new List<BallInstance>();
            if (!_ballAreaPerContainer.ContainsKey(containerInstance))
                CreateNewBallAreaReference(containerInstance);
        }
        
        private FloatReference CreateNewBallAreaReference(ContainerInstance containerInstance)
        {
            FloatReference newAreaRef = new FloatReference
                { UseConstant = false, Variable = ScriptableObject.CreateInstance<FloatVariable>() };
            _ballAreaPerContainer[containerInstance] = newAreaRef;
            return newAreaRef;
        }
    }
}