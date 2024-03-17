using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Cannon
{
    public class CannonTracker : ItemTracker<CannonInstance>
    {
        #region Singleton

        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static CannonTracker Instance => _instance ??= new CannonTracker();

        private static CannonTracker _instance;

        private CannonTracker()
        {
        }

        
        private void Awake()
        {
            _instance = this;
        }
        #endregion

        public override void ClearItem(CannonInstance item)
        {
            item.DestroyCurrentBall();
            base.ClearItem(item);
        }
    }

    public class CannonTrackerInformation : ItemInformation<CannonInstance>
    {
        public CannonTrackerInformation(CannonInstance item, List<int> playerIndex = null) : base(item, playerIndex)
        {
        }
    }
}