using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerPart : MonoBehaviour
{
    [SerializeField] public Collider2D collider2D;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public ContainerSide containerSide;
    public enum ContainerSide
    {
        Left,
        Right,
        Bottom
    }
}
