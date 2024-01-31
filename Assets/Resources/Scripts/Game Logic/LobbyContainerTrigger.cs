using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class LobbyContainerTrigger : MonoBehaviour
{
    // Trash code, but for now it'll get the job done to test a few things
    [Range(1, 4)][SerializeField] public int playerNumberThreshold;
    [SerializeField] private GameObject _textEnoughPlayers;
    [SerializeField] private GameObject _lidCollider;
    
    private Collider2D _collider;
    public UnityEvent OnTrigger;

    private void OnEnable()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void UpdateContainerBehavior(int playersNumber)
    {
        _lidCollider.SetActive(playersNumber < playerNumberThreshold);
        _textEnoughPlayers.SetActive(playersNumber >= playerNumberThreshold);
        _collider.enabled = playersNumber >= playerNumberThreshold;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
            OnTrigger?.Invoke();
        }
    }
}
