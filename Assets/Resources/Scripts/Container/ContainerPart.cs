using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerPart : MonoBehaviour
{
    [SerializeField] public List<Collider2D> colliders2D;
    [SerializeField] public ContainerSide containerSide;
    public enum ContainerSide
    {
        Left,
        Right,
        Bottom
    }
}
