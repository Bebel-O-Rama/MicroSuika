using System;
using System.Collections;
using System.Collections.Generic;
using MultiSuika.Container;
using UnityEngine;

namespace MultiSuika.Skin
{
    public class SpeedMeterSkin : MonoBehaviour
    {
        [SerializeField] private List<Texture2D> _playerSpeedShaderMasks;
        
        private int _playerIndex;
        private ContainerInstance _containerInstance;

        private Material _containerSideMat;

        private void Awake()
        {
            _containerSideMat = GetComponent<SpriteRenderer>().material;
        }

        void Start()
        {
            _containerInstance = GetComponentInParent<ContainerInstance>();
            _playerIndex = ContainerTracker.Instance.GetPlayerFromItem(_containerInstance);
            _containerSideMat.SetTexture("_ShaderMask", _playerSpeedShaderMasks[_playerIndex]);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}