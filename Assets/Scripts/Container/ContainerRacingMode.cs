using System.Collections.Generic;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.DebugInfo;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerRacingMode : ContainerInstance
    {
        private int _playerIndex;
        [SerializeField] private List<SignalCollider2D> _hurtboxes; 
        
        private void Start()
        {
            _playerIndex = ContainerTracker.Instance.GetPlayersByItem(this).First();

            foreach (var hurtbox in _hurtboxes)
            {
                hurtbox.SubscribeTriggerEnter2D(HurtboxTriggered);
            }
        }
        
        private void HurtboxTriggered(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Ball"))
                return;
            var ball = other.GetComponentInParent<BallInstance>();
            ContainerTracker.Instance.OnContainerHit.CallAction((ball, this), _playerIndex);
            ball.ClearBall(false);
        }
        
        #region Setter
        
        public void SetLayer(string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            if (layer < 0)
                return;
            transform.parent.transform.SetLayerRecursively(layer);
        }
        
        #endregion
    }
}
