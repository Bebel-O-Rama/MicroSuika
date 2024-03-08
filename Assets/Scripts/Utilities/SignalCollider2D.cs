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
        
        private Action<Collider2D> _onTriggerEnter;
        private Action<Collider2D> _onTriggerStay;
        private Action<Collider2D> _onTriggerExit;

        public void SubscribeCollision2DEnter(Action<Collision2D> method) => _onCollisionEnter += method;
        public void UnsubscribeCollision2DEnter(Action<Collision2D> method) => _onCollisionEnter -= method;
        public void SubscribeCollision2DStay(Action<Collision2D> method) => _onCollisionStay += method;
        public void UnsubscribeCollision2DStay(Action<Collision2D> method) => _onCollisionStay -= method;
        public void SubscribeCollision2DExit(Action<Collision2D> method) => _onCollisionExit += method;
        public void UnsubscribeCollision2DExit(Action<Collision2D> method) => _onCollisionExit -= method;
        
        public void SubscribeTriggerEnter2D(Action<Collider2D> method) => _onTriggerEnter += method;
        public void UnsubscribeTriggerEnter2D(Action<Collider2D> method) => _onTriggerEnter -= method;
        public void SubscribeTriggerStay2D(Action<Collider2D> method) => _onTriggerStay += method;
        public void UnsubscribeTriggerStay2D(Action<Collider2D> method) => _onTriggerStay -= method;
        public void SubscribeTriggerExit2D(Action<Collider2D> method) => _onTriggerExit += method;
        public void UnsubscribeTriggerExit2D(Action<Collider2D> method) => _onTriggerExit -= method;
        
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            _onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _onTriggerExit?.Invoke(other);
        }
    }
}
