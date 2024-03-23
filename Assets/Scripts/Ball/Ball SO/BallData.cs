using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Ball
{
    [CreateAssetMenu(menuName = "Ball/Ball Data")]
    public class BallData : ScriptableObject
    {
        [Tooltip("The smaller the index, the smaller the ball should be. The index of the smallest ball should be 0")]
        [SerializeField] [Min(0)] private int _index;
        [SerializeField] private float _scale;
        [SerializeField] public FloatReference _mass; // 1, 0.1
        [Tooltip("1 is the baseline")] 
        [SerializeField] [Range(0f, 4f)] private float _spawnChance = 1f;
        
        public int Index { get => _index; }
        public float Scale { get => _scale; }
        public float Mass { get => _mass; }
        public float SpawnChance { get => _spawnChance; }
        

        // Triangular number sequence
        public int GetScoreValue() => (Index+1) * (Index + 2) / 2;
    }
}
