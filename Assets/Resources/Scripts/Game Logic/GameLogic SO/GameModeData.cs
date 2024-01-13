using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Mode Data")]
public class GameModeData : ScriptableObject
{
    [Header("Cannon Parameters")]
    public CannonData cannonData;
    public List<CannonSpawnPositionData> cannonSpawnPositionData;
    public bool isMainCannon = true;

    [Header("Ball Parameters")]
    public BallSetData ballSetData;
    
    [Header("Player Parameters")]
    public float cooldownBeforeInputConnexion;
}
