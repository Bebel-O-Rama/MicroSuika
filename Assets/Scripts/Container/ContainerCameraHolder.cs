using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerCameraHolder : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _mainVerticalTransform;
        [SerializeField] private Transform _secondaryTransform;

        private void Start()
        {
            _camera.cullingMask |= (1 << gameObject.layer);
        }
            
        public void SetMainVerticalPosition(float yPos) => _mainVerticalTransform.position = new Vector3(0, yPos, 0);

        public float GetMainVerticalPosition() => _mainVerticalTransform.position.y;
        
    }
}