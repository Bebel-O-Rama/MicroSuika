using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// For now I'm not using that SO. I'll check that later on if it really becomes necessary to manually add bounce on fusion (who knows, it might even appear by itself) 
/// </summary>
public class FusingImpactParameter : ScriptableObject
{
    public AnimationCurve baseForceByRadius;
    public AnimationCurve forceMultiplierByDistance;
    
    public LayerMask layerMask;

    public void ApplyExpansionForce(Vector2 impactPosition, float minRadius, float maxRadius, int newBallTier)
    {
        // var ballInRange = Physics2D.OverlapCircleAll(new Vector2(impactPosition.x, impactPosition.y), radius, layerMask);
        
        
        // Faire un overlapCircle pour trouver toutes les balles dans le radius de la nouvelle balle
        var ballInRange = Physics2D.OverlapCircleAll(impactPosition, maxRadius, layerMask);
        
        foreach (var ballCollider2D in ballInRange)
        {
            var ball = ballCollider2D.GetComponent<Ball>();
            // Clean les colliders pour uniquement garder les balles qui n'ont pas le même tier que le newBallTier (ça va fuse avec direct, on ne veut pas les pitcher)
            if (ball.GetBallTier() != newBallTier)
            {
                // Applique un impact sur chaque balle normalisant la force selon la distance (l'animation curve établi le % de la force à être appliqué selon la distance entre le point d'impact pour la balle et l'explosion (on normalize avec le max possible)
                float pushDistance = Mathf.Abs(Vector2.Distance(impactPosition, ball.GetBallPosition()) - maxRadius - ball.GetBallRadius());
                Debug.Log($"pushDistance with {ball.gameObject.name} is : {pushDistance}");
            }
        }
        
        // Ajouter ce SO au BallSetData et le référencer dans Ball.cs lorsqu'une fusion est faite

        // Ensuite, debug! (pretty sure que ça va casser de partout)
    }
}
