using System;
using UnityEngine;

namespace MultiSuika.Utilities
{
    [RequireComponent(typeof(Collider2D))]
    public class SignalCollider2D : MonoBehaviour
    {
        public ActionMethod<Collision2D> OnCollision2DEnter { get; } = new ActionMethod<Collision2D>();
        public ActionMethod<Collision2D> OnCollision2DStay { get; } = new ActionMethod<Collision2D>();
        public ActionMethod<Collision2D> OnCollision2DExit { get; } = new ActionMethod<Collision2D>();
        
        public ActionMethod<Collider2D> OnTrigger2DEnter { get; } = new ActionMethod<Collider2D>();
        public ActionMethod<Collider2D> OnTrigger2DStay { get; } = new ActionMethod<Collider2D>();
        public ActionMethod<Collider2D> OnTrigger2DExit { get; } = new ActionMethod<Collider2D>();

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollision2DEnter.CallAction(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            OnCollision2DStay.CallAction(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            OnCollision2DExit.CallAction(other);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTrigger2DEnter.CallAction(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTrigger2DStay.CallAction(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTrigger2DExit.CallAction(other);
        }
    }
}
