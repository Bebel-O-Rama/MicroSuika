using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cannon/Cannon Spawn Position Data")]
public class CannonSpawnPositionData : ScriptableObject
{
    public Vector2 centerPosition;
    [Min(0f)] public float maxHorizontalDelta;
    [Min(0f)] public float xRandomSpawnRangeDelta;
}
