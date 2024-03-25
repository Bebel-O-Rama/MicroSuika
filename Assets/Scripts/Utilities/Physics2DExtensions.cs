using System.Linq;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public static class Physics2DExtensions
    {
        public static void ApplyCircularImpulse(float radius, Vector2 position, string tag = "", float impulseForcePerUnit = 3f,
            float impulseExpPower = 2.3f)
        {
            var objectsInRange =
                from raycast in Physics2D.CircleCastAll(position, radius, Vector2.zero, Mathf.Infinity)
                select raycast.collider;

            foreach (var obj in objectsInRange)
            {
                if (tag != "" && !obj.CompareTag(tag))
                    continue;
                Vector2 pushDirection = obj.transform.position - (Vector3)position;
                pushDirection.Normalize();

                var pushIntensity =
                    Mathf.Pow(
                        Mathf.Abs(radius - Vector2.Distance(obj.ClosestPoint(position), position)) *
                        impulseForcePerUnit, impulseExpPower);

                obj.GetComponentInParent<Rigidbody2D>()?.AddForce(pushIntensity * pushDirection, ForceMode2D.Impulse);
            }
        }
        
        public static void ApplyCircularImpulse(float radius, Vector2 position, LayerMask layerMask, float impulseForcePerUnit = 3f,
            float impulseExpPower = 2.3f)
        {
            var objectsInRange =
                from raycast in Physics2D.CircleCastAll(position, radius, Vector2.zero, Mathf.Infinity, layerMask)
                select raycast.collider;

            foreach (var obj in objectsInRange)
            {
                Vector2 pushDirection = obj.transform.position - (Vector3)position;
                pushDirection.Normalize();

                var pushIntensity =
                    Mathf.Pow(
                        Mathf.Abs(radius - Vector2.Distance(obj.ClosestPoint(position), position)) *
                        impulseForcePerUnit, impulseExpPower);

                obj.GetComponentInParent<Rigidbody2D>()?.AddForce(pushIntensity * pushDirection, ForceMode2D.Impulse);
            }
        }
    }
}