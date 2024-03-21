using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MultiSuika.Utilities;

namespace MultiSuika.Cannon
{
    public class CannonTracker : ItemTracker<CannonInstance, CannonTrackerInformation>
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
        
        protected override CannonTrackerInformation CreateInformationInstance(CannonInstance item, List<int> playerIndex)
        {
            return new CannonTrackerInformation(item, playerIndex);
        }

        public override void ClearItem(CannonInstance item)
        {
            item.DestroyCurrentBall();
            base.ClearItem(item);
        }
    }

    public class CannonTrackerInformation : ItemInformation<CannonInstance>
    {
        public CannonTrackerInformation(CannonInstance item, List<int> playerIndex) : base(item, playerIndex)
        {
        }
    }
}