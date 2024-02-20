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
        public List<BallData> ballSetData;
        public Ball ballPrefab;
    
        [Header("Impulse parameters")]
        public FloatReference impulseMultiplier;
        public FloatReference impulseExpPower;
        public FloatReference impulseRangeMultiplier;
    
        [Header("Physic parameters")]
        public FloatReference bounciness;
        public FloatReference friction;
        public FloatReference gravityScale;
    
    
        private float _totalWeight;
        
        public BallData GetBallData(int index) => ballSetData.Count > index ? ballSetData[index] : null;
        public int GetMaxTier => ballSetData.Count - 1;
    
        public int GetRandomBallTier(bool usingWeight = true)
        {
            if (!usingWeight)
                return Random.Range(0, ballSetData.Count - 1);
        
            float randValue = Random.Range(0f, 1f);
            foreach (var ballData in ballSetData)
            {
                randValue -= ballData.spawnChance / _totalWeight;
                if (randValue <= 0)
                    return ballData.index;
            }
            return 0;
        }

        private void TestingAndCleaningSet()
        {
            // It's not flawless, but at least it takes care of null elements and duplicates. The OnValidate should already take care of the order of the BallData
            var tempSet = new List<BallData>();
            foreach (var ballData in ballSetData)
            {
                if (ballData != null && !tempSet.Contains(ballData))
                    tempSet.Add(ballData);
            }
            ballSetData = tempSet;
        }

        private void SetWeight() => _totalWeight = ballSetData.Sum(b => b.spawnChance);

        private void OnValidate()
        {
            ballSetData.RemoveAll(item => item == null);
            ballSetData = ballSetData.OrderBy(ball => ball.index).ToList();
        }

        private void OnEnable()
        {
            TestingAndCleaningSet();
            SetWeight();
        }
    }
}