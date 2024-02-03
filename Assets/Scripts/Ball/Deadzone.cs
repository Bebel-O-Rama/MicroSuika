using UnityEngine;

namespace MultiSuika.Ball
{
    [RequireComponent(typeof(Collider2D))]
    public class Deadzone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ball"))
                other.GetComponent<Ball>().ClearBall(false);
        }
    }
}
