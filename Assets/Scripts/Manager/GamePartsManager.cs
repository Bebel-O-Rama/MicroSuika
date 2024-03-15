using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Container;
using UnityEngine;

namespace MultiSuika.Manager
{
    public class GamePartsManager : MonoBehaviour
    {
        #region Singleton

        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static GamePartsManager Instance => _instance ??= new GamePartsManager();

        private static GamePartsManager _instance;

        private GamePartsManager()
        {
        }

        #endregion

        private ContainerTracker _containerTracker;

        // TODO: Check if that's necessary
        public void Awake()
        {
            _containerTracker = new ContainerTracker();
        }
        
        



    }



}