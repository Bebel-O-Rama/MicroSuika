using System;
using UnityEngine;

namespace MultiSuika.Utilities
{
    [RequireComponent(typeof(Collider2D))]
    public class SignalCollider2D : MonoBehaviour
    {
        private Action<Collision2D> _onTriggerEnter;
        private Action<Collision2D> _onTriggerStay;
        private Action<Collision2D> _onTriggerExit;

        private Action<Collision2D> _onCollisionEnter;
        private Action<Collision2D> _onCollisionStay;
        private Action<Collision2D> _onCollisionExit;

        public void SubscribeCollider2DEnter(Action<Collision2D> method, bool isTrigger)
        {
            if (isTrigger)
                _onTriggerEnter += method;
            else
                _onCollisionEnter += method;
        }

        public void UnsubscribeCollider2DEnter(Action<Collision2D> method, bool isTrigger)
        {
            if (isTrigger)
                _onTriggerEnter -= method;
            else
                _onCollisionEnter -= method;
        }

        public void SubscribeCollider2DStay(Action<Collision2D> method, bool isTrigger)
        {
            if (isTrigger)
                _onTriggerStay += method;
            else
                _onCollisionStay += method;
        }

        public void UnsubscribeCollider2DStay(Action<Collision2D> method, bool isTrigger)
        {
            if (isTrigger)
                _onTriggerStay -= method;
            else
                _onCollisionStay -= method;
        }

        public void SubscribeCollider2DExit(Action<Collision2D> method, bool isTrigger)
        {
            if (isTrigger)
                _onTriggerExit += method;
            else
                _onCollisionExit += method;
        }

        public void UnsubscribeCollider2DExit(Action<Collision2D> method, bool isTrigger)
        {
            if (isTrigger)
                _onTriggerExit -= method;
            else
                _onCollisionExit -= method;
        }


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