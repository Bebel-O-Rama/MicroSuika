using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerInputHandler inputHandler;
    // Start is called before the first frame update

    public PlayerInputHandler GetInputHandler() => inputHandler;
}
