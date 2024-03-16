using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Ball;
using MultiSuika.Cannon;
using MultiSuika.Container;
using MultiSuika.GameLogic;
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

        public readonly ContainerTracker ContainerTracker = new ContainerTracker();
        public readonly CannonTracker CannonTracker = new CannonTracker();
        public readonly BallTracker BallTracker = new BallTracker();

        private void Awake()
        {
            _instance = this;
            Init();
        }

        private void Init()
        {
        }
    }
}