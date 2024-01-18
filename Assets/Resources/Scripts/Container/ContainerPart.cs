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

    // public void AdjustContainerSide(float horizontalLength, float verticalLength)
    // {
    //     Vector2 positionModifier = Vector2.zero;
    //     Vector2 sizeModifier = Vector2.one;
    //     switch (containerSide)
    //     {
    //         case ContainerSide.Left:
    //             positionModifier = new Vector2(-horizontalLength / 2f, 0f);
    //             sizeModifier = new Vector2(1f, verticalLength);
    //             break;
    //         case ContainerSide.Right:
    //             positionModifier = new Vector2(horizontalLength / 2f, 0f);
    //             sizeModifier = new Vector2(1f, verticalLength);
    //             break;
    //         case ContainerSide.Bottom:
    //             positionModifier = new Vector2(0f, - verticalLength / 2f);
    //             sizeModifier = new Vector2(horizontalLength, 1f);
    //             break;
    //     }
    //     transform.position = positionModifier;
    //     collider2D.transform.localScale = sizeModifier;
    // }
}
