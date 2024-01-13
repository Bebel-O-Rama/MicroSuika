using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CannonSpawnPositionData : ScriptableObject
{
    public Vector2 centerPosition;
    [Min(0f)] public float maxHorizontalDelta;
    [Min(0f)] public float xRandomSpawnRangeDelta;
    
    // public (Vector2 pos, float delta, float xRand) GetCannonSpawnPositionData() => (centerPosition, maxHorizontalDelta, xRandomSpawnRangeDelta);
}
