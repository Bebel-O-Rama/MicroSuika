using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiSuika.DebugInfo
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallImpulseDebug : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private float _impactForce;

        [SerializeField] private Color _ready;
        [SerializeField] private Color _impulse;
        [SerializeField] private float _cooldown = 1f;

        [SerializeField] private LayerMask _layerMask;

        private Vector3 _mousePosition;
        private bool _isReady;
        private Color _currentWireColor;
        private Camera _mainCam;

        private void Start()
        {
            _mainCam = Camera.main;
            _currentWireColor = _ready;
            _isReady = true;
        }

        private void Update()
        {
            _mousePosition = _mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _mousePosition.z = 0;
            if (Mouse.current.leftButton.wasPressedThisFrame && _isReady)
                StartCoroutine(AddImpulse());
        }

        private IEnumerator AddImpulse()
        {
            _isReady = false;
            _currentWireColor = _impulse;

            var ballsInRange =
                from raycast in Physics2D.CircleCastAll(_mousePosition, _radius, Vector2.zero, Mathf.Infinity, _layerMask)
                select raycast.collider;

            foreach (var ball in ballsInRange)
            {
                Vector2 pushDirection = ball.transform.position - _mousePosition;
                pushDirection.Normalize();

                float pushLength = Mathf.Abs(_radius - Vector2.Distance(ball.ClosestPoint(_mousePosition), _mousePosition));

                ball.GetComponent<Rigidbody2D>().AddForce(pushLength * _impactForce * pushDirection, ForceMode2D.Impulse);

            }

            yield return new WaitForSeconds(_cooldown);
            _currentWireColor = _ready;
            _isReady = true;
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = _currentWireColor;
            Gizmos.DrawWireSphere(_mousePosition, _radius);
        }
    }
}