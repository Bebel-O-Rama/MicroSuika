using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ContainerInitializationData : ScriptableObject
{
    [Header("Container Prefab")] 
    public GameObject containerPrefab;
    
    [Tooltip("This distances goes from one container center point to the other")]
    public List<Vector2> leftmostContainerPositions;

    [Header("Container Parent Scaling Parameters")]
    public List<float> containerGeneralScaling;
}
