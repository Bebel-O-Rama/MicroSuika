using System.Collections.Generic;
using System.Linq;
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

        public ActionMethodPlayerWrapper<(BallInstance, ContainerInstance)> OnContainerHit { get; } =
            new ActionMethodPlayerWrapper<(BallInstance, ContainerInstance)>();

        protected override ContainerTrackerInformation CreateInformationInstance(ContainerInstance item,
            List<int> playerIndex)
        {
            return new ContainerTrackerInformation(item, playerIndex);
        }

        public Transform GetParentTransformFromPlayer(int playerIndex) =>
            GetItemsByPlayer(playerIndex).First().ContainerParent.transform;
    }

    public class ContainerTrackerInformation : ItemInformation<ContainerInstance>
    {
        public ActionMethodPlayerWrapper<(BallInstance, ContainerInstance)> OnContainerHit { get; } =
            new ActionMethodPlayerWrapper<(BallInstance, ContainerInstance)>();

        public ContainerTrackerInformation(ContainerInstance item, List<int> playerIndex) : base(item, playerIndex)
        {
        }
    }
}