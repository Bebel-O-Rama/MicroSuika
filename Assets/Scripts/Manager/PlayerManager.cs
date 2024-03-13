using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace MultiSuika.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton
        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static PlayerManager Instance => _instance ??= new PlayerManager();
        private static PlayerManager _instance;
        
        private PlayerManager()
        {
        }
        #endregion
        private Dictionary<>
        private void Awake()
        {
            _instance = this;
            InitializeManager();
        }

        private void InitializeManager()
        {
            
        }
    }
}
