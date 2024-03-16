using System.Collections.Generic;
using System.Linq;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerTracker
    {
        private readonly List<ContainerTrackerInformation>
            _containerTrackerInformation = new List<ContainerTrackerInformation>();

        public void AddNewContainer(ContainerInstance container)
        {
            var containerInfo = GetInformationFromContainer(container);
            if (containerInfo == null)
                _containerTrackerInformation.Add(new ContainerTrackerInformation(container));
        }

        public void ConnectPlayerToContainer(ContainerInstance container, int playerIndex)
        {
            var containerInfo = GetInformationFromContainer(container);

            if (containerInfo != null && containerInfo.ContainsPlayerIndex(playerIndex))
                return;

            if (containerInfo == null)
            {
                _containerTrackerInformation.Add(new ContainerTrackerInformation(container, playerIndex));
                return;
            }

            containerInfo.SetPlayerIndex(playerIndex);
        }

        public void ClearContainers()
        {
            for (int i = _containerTrackerInformation.Count - 1; i >= 0; i--)
                ClearContainer(_containerTrackerInformation[i].ContainerInstance);
        }

        public void ClearContainer(ContainerInstance container)
        {
            var info = _containerTrackerInformation.FirstOrDefault(info =>
                info.ContainerInstance.GetInstanceID() == container.GetInstanceID());

            Object.Destroy(container.gameObject);

            if (info != null)
                _containerTrackerInformation.Remove(info);
        }

        public ContainerInstance GetContainerFromPlayer(int playerIndex) =>
            _containerTrackerInformation.FirstOrDefault(
                c => c.ContainsPlayerIndex(playerIndex))?.ContainerInstance;

        public ContainerInstance GetContainerByIndex(int index) =>
            _containerTrackerInformation.GetElementAtIndexOrDefault(index)?.ContainerInstance;
            
        
        private List<int> GetPlayersFromContainer(ContainerInstance container) => _containerTrackerInformation
            .FirstOrDefault(c => c.ContainerInstance == container)?.PlayerIndexes;

        private bool IsContainerRegistered(ContainerInstance container) =>
            GetInformationFromContainer(container) != null;

        private ContainerTrackerInformation GetInformationFromContainer(ContainerInstance container) =>
            _containerTrackerInformation.FirstOrDefault(c =>
                c.ContainerInstance.GetInstanceID() == container.GetInstanceID());
    }

    public class ContainerTrackerInformation
    {
        public ContainerInstance ContainerInstance { get; }
        public List<int> PlayerIndexes { get; }

        public ContainerTrackerInformation(ContainerInstance containerInstance, int playerIndex = -1,
            List<int> playerIndexes = null)
        {
            if (playerIndex != -1 && playerIndexes != null)
                Debug.LogError("Only one of playerIndex or playerIndexes can be specified.");


            ContainerInstance = containerInstance;

            PlayerIndexes = new List<int>();
            if (playerIndex >= 0)
                PlayerIndexes.Add(playerIndex);
            else if (playerIndexes != null)
                PlayerIndexes = new List<int>(playerIndexes);
        }

        public void SetPlayerIndex(int playerIndex, bool isAdding = true) =>
            PlayerIndexes.SetInList(playerIndex, isAdding);

        public bool ContainsPlayerIndex(int playerIndex) => PlayerIndexes.Contains(playerIndex);
    }
}