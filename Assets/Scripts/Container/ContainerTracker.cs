using MultiSuika.Ball;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerTracker : ItemTracker<ContainerInstance, ContainerTrackerInformation>
    {
        #region Singleton

        public static ContainerTracker Instance => _instance ??= new ContainerTracker();

        private static ContainerTracker _instance;

        private ContainerTracker()
        {
        }

        #endregion

        public ActionMethodPlayerWrapper<BallInstance> OnContainerHit { get; } =
            new ActionMethodPlayerWrapper<BallInstance>();

        public Transform GetParentTransformFromPlayer(int playerIndex) =>
            GetItemFromPlayerOrDefault(playerIndex).ContainerParent.transform;

        protected override ContainerTrackerInformation CreateInformationInstance(ContainerInstance item, int playerIndex)
        {
            return new ContainerTrackerInformation(item, playerIndex);
        }
    }

    public class ContainerTrackerInformation : ItemInformation<ContainerInstance>
    {
        public ContainerTrackerInformation(ContainerInstance item, int playerIndex) : base(item, playerIndex)
        {
        }
    }
}