using System.Collections.Generic;
using System.Linq;
using MultiSuika.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MultiSuika.Ball
{
    [CreateAssetMenu(menuName = "Ball/Ball Set Data")]
    public class BallSetData : ScriptableObject
    {
        [Header("The smaller the index position of the ball, the smaller its radius should be")]
        [Header("ALSO, you can't have null here, you need to drag and drop the BallData SO on the list")]
        [SerializeField] private List<BallData> _ballSet;
        [SerializeField] private BallInstance _ballInstancePrefab;
    
        [Header("Impulse parameters")]
        [SerializeField] private FloatReference _impulseForcePerUnit; // 3, 0.05
        [SerializeField] private FloatReference _impulseExpPower; // 2.3, 0.15
        [SerializeField] private FloatReference _impulseRangeMultiplier; // 1.2, 0.1
    
        [Header("Physic parameters")]
        [SerializeField] private FloatReference _bounciness; //0.1, 0.1
        [SerializeField] private FloatReference _friction; // 0.3, 0.1
        [SerializeField] private FloatReference _gravityScale; // 1, 0.1
    
        public BallInstance BallInstancePrefab { get => _ballInstancePrefab; }
        public FloatReference ImpulseForcePerUnit { get => _impulseForcePerUnit; }
        public FloatReference ImpulseExpPower { get => _impulseExpPower; }
        public FloatReference ImpulseRangeMultiplier { get => _impulseRangeMultiplier; }
        public FloatReference Bounciness { get => _bounciness; }
        public FloatReference Friction { get => _friction; }
        public FloatReference GravityScale { get => _gravityScale; }
        
        private float _totalWeight;
        
        public BallData GetBallData(int index) => _ballSet.Count > index ? _ballSet[index] : null;
        public int GetMaxTier => _ballSet.Count - 1;
    
        private void OnEnable()
        {
            TestingAndCleaningSet();
            SetWeight();
        }
        
        public int GetRandomBallTier(bool usingWeight = true)
        {
            if (!usingWeight)
                return Random.Range(0, _ballSet.Count - 1);
        
            var randValue = Random.Range(0f, 1f);
            foreach (var ballData in _ballSet)
            {
                randValue -= ballData.SpawnChance / _totalWeight;
                if (randValue <= 0)
                    return ballData.Index;
            }
            return 0;
        }

        // It's not flawless, but at least it takes care of null elements and duplicates.
        // The OnValidate should already take care of the order of the BallData
        #region Editor

        private void TestingAndCleaningSet()
        {
            var tempSet = new List<BallData>();
            foreach (var ballData in _ballSet)
            {
                if (ballData != null && !tempSet.Contains(ballData))
                    tempSet.Add(ballData);
            }
            _ballSet = tempSet;
        }

        private void SetWeight() => _totalWeight = _ballSet.Sum(b => b.SpawnChance);

        private void OnValidate()
        {
            _ballSet.RemoveAll(item => item == null);
            _ballSet = _ballSet.OrderBy(ball => ball.Index).ToList();
        }

        #endregion
    }
}