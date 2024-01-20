using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] public ContainerPart leftPart;
    [SerializeField] public ContainerPart rightPart;
    [SerializeField] public ContainerPart bottomPart;
    
    [SerializeField] [Min(0f)] public float horizontalMvtHalfLength;
    
    private GameObject _containerParent;
    
    public GameObject containerParent
    {
        get => _containerParent;
        set
        {
            _containerParent = value;
            transform.SetParent(_containerParent.transform);
        }
    }

    
    public float GetContainerHorizontalHalfLength() => horizontalMvtHalfLength;
}
