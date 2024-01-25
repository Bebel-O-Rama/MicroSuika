using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skin/Player Skin Data")]
public class PlayerSkinData : ScriptableObject
{
    // Container skin parameters
    [Header("----- CONTAINER -----")] 
    public Sprite containerBackground;
    
    // Cannon skin parameters
    [Header("----- CANNON -----")]
    public Sprite cannonSprite;

    // Ball skin parameters
    [Header("----- BALL -----")] 
    public BallSpriteThemeData ballTheme;
    
    // Ball skin parameters
    [Header("----- OTHER -----")] 
    public Color baseColor;
}
