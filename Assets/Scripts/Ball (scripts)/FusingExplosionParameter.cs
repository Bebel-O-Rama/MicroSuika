using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusingExplosionParameter : ScriptableObject
{
    public AnimationCurve ratioForceDistance;
    [Min(0f)] public float baseImpulseForce;

    public void ApplyExplosionForce(Vector3 impactPosition, float radius, int newBallTier)
    {
        // Faire un overlapCircle pour trouver toutes les balles dans le radius de la nouvelle balle
        // Clean les colliders pour uniquement garder les balles qui n'ont pas le même tier que le newBallTier (ça va fuse avec direct, on ne veut pas les pitcher)
        // Applique un impact sur chaque balle normalisant la force selon la distance (l'animation curve établi le % de la force à être appliqué selon la distance entre le point d'impact pour la balle et l'explosion (on normalize avec le max possible)
        // Ajouter ce SO au BallSetData et le référencer dans Ball.cs lorsqu'une fusion est faite
        
        // Ensuite, debug! (pretty sure que ça va casser de partout)
    }
}
