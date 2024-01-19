using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] public ContainerPart leftPart;
    [SerializeField] public ContainerPart rightPart;
    [SerializeField] public ContainerPart bottomPart;
    
    public GameObject containerParent
    {
        get => _containerParent;
        set
        {
            _containerParent = value;
            transform.SetParent(_containerParent.transform);
        }
    }

    private GameObject _containerParent;
    
    public float GetContainerHorizontalLength() => bottomPart.collider2D.transform.localScale.x;
}
