using System.Collections.Generic;
using MultiSuika.Manager;
using MultiSuika.UI;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Skin
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private List<BackgroundMovement> _backgroundMovements;
        [SerializeField] private AnimationCurve _multiplierProgression;
        [SerializeField] [Min(1f)] private float _topAverageSpeed;

        void LateUpdate()
        {
            var averageSpeed = ScoreManager.Instance.GetAverageSpeedReference();
            var movementCoefficient = _multiplierProgression.EvaluateClamp(averageSpeed / _topAverageSpeed);
            foreach (var backgroundMovement in _backgroundMovements)
            {
                backgroundMovement.UpdateMovementSpeedByMulti(movementCoefficient);
            }
        }
    }
}