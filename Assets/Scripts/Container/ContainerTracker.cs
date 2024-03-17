using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MultiSuika.Utilities;

namespace MultiSuika.Container
{
    public class ContainerTracker : ItemTracker<ContainerInstance, ContainerTrackerInformation>
    {
        #region Singleton

        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static ContainerTracker Instance => _instance ??= new ContainerTracker();

        private static ContainerTracker _instance;

        private ContainerTracker()
        {
        }

        private void Awake()
        {
            _instance = this;
        }
        #endregion

        protected override ContainerTrackerInformation CreateInformationInstance(ContainerInstance item, List<int> playerIndex)
        {
            return new ContainerTrackerInformation(item, playerIndex);
        }
    }

    public class ContainerTrackerInformation : ItemInformation<ContainerInstance>
    {
        public ContainerTrackerInformation(ContainerInstance item, List<int> playerIndex) : base(item, playerIndex)
        {
        }
    }
}