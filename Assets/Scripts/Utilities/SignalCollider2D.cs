using System;
using UnityEngine;

namespace MultiSuika.Utilities
{
    [RequireComponent(typeof(Collider2D))]
    public class SignalCollider2D : MonoBehaviour
    {
        private Action<Collision2D> _onCollisionEnter;
        private Action<Collision2D> _onCollisionStay;
        private Action<Collision2D> _onCollisionExit;

        public void SubscribeCollisionEnter2D(Action<Collision2D> method) => _onCollisionEnter += method;
        public void UnsubscribeCollisionEnter2D(Action<Collision2D> method) => _onCollisionEnter -= method;
        
        public void SubscribeCollisionStay2D(Action<Collision2D> method) => _onCollisionStay += method;
        public void UnsubscribeCollisionStay2D(Action<Collision2D> method) => _onCollisionStay -= method;
        
        public void SubscribeCollisionExit2D(Action<Collision2D> method) => _onCollisionExit += method;
        public void UnsubscribeCollisionExit2D(Action<Collision2D> method) => _onCollisionExit -= method;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            _onCollisionEnter?.Invoke(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            _onCollisionStay?.Invoke(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            _onCollisionExit?.Invoke(other);
        }
    }
}
