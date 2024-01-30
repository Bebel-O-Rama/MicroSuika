using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Deadzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
            Destroy(other.gameObject);
    }
}
