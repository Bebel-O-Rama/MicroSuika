using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game Logic/Game Mode Data")]
public class GameModeData : ScriptableObject
{
    [Header("Cannon Parameters")]
    public CannonInitializationData cannonInitializationData;

    [Header("Ball Parameters")]
    public BallSetData ballSetData;
    
    [Header("Player Parameters")]
    public float cooldownBeforeInputConnexion;
    
    [Header("Container Parameters")]
    public ContainerInitializationData containerInitializationData;
}
