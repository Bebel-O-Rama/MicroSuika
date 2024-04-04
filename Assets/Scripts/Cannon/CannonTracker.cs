using MultiSuika.Utilities;

namespace MultiSuika.Cannon
{
    public class CannonTracker : ItemTracker<CannonInstance, CannonTrackerInformation>
    {
        #region Singleton

        public static CannonTracker Instance => _instance ??= new CannonTracker();

        private static CannonTracker _instance;

        private CannonTracker()
        {
        }

        #endregion

        protected override CannonTrackerInformation CreateInformationInstance(CannonInstance item, int playerIndex)
        {
            return new CannonTrackerInformation(item, playerIndex);
        }

        public override void ClearItem(CannonInstance item)
        {
            item.ClearCannon();
            base.ClearItem(item);
        }
    }

    public class CannonTrackerInformation : ItemInformation<CannonInstance>
    {
        public CannonTrackerInformation(CannonInstance item, int playerIndex) : base(item, playerIndex)
        {
        }
    }
}