using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Mode Data")]
public class GameModeData : ScriptableObject
{
    [Header("Cannon Parameters")]
    public CannonData cannonData;
    public List<Vector2> cannonCenterPosition;
    [Min(0f)] public float maxHorizontalDelta;
    public bool isMainCannon = true;
    public bool spawnCannonAtRandomXPos = false;

    [Header("Ball Parameters")]
    public BallSetData ballSetData;
    
    [Header("Player Parameters")]
    public float cooldownBeforeInputConnexion;
}
