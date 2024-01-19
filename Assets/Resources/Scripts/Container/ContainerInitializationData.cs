using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ContainerInitializationData : ScriptableObject
{
    [Header("Container Prefab")] 
    public GameObject containerPrefab;

    [Header("Container Scaling and Position Parameters")]
    [Tooltip("This distances goes from one container center point to the other")]
    public List<Vector2> leftmostContainerPositions;
    public List<float> containerGeneralScaling;
    
    [Header("Other Parameters")]
    public bool usingSingleContainer = false;
}
