using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [Range(0f, 2 * Mathf.PI)] [SerializeField]
    public float movementDirectionAngle;

    [Min(0f)] [SerializeField] public float movementSpeed;
    static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private Renderer _renderer;
    private Vector2 _directionalMovement;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _directionalMovement = new Vector2(Mathf.Cos(movementDirectionAngle), Mathf.Sin(movementDirectionAngle));
    }

    private void LateUpdate()
    {
        _directionalMovement = new Vector2(Mathf.Cos(movementDirectionAngle), Mathf.Sin(movementDirectionAngle));

        var currentOffset = _renderer.material.GetTextureOffset(MainTex);
        var newOffset = currentOffset + (movementSpeed / 15f) * Time.deltaTime * _directionalMovement;
        _renderer.material.SetTextureOffset(MainTex, newOffset);
    }
}