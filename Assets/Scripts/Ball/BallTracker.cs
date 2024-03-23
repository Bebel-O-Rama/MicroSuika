using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MultiSuika.Utilities;

namespace MultiSuika.Ball
{
    public class BallTracker : ItemTracker<BallInstance, BallTrackerInformation>
    {
        #region Singleton

        public static BallTracker Instance => _instance ??= new BallTracker();

        private static BallTracker _instance;

        private BallTracker()
        {
        }

        private void Awake()
        {
            _instance = this;
        }

        #endregion

        public ActionMethodPlayerWrapper<BallInstance> OnBallFusion { get; } =
            new ActionMethodPlayerWrapper<BallInstance>();
        
        protected override BallTrackerInformation CreateInformationInstance(BallInstance item, List<int> playerIndex)
        {
            return new BallTrackerInformation(item, playerIndex);
        }
    }

    public class BallTrackerInformation : ItemInformation<BallInstance>
    {
        public BallTrackerInformation(BallInstance item, List<int> playerIndex) : base(item, playerIndex)
        {
        }
    }
}