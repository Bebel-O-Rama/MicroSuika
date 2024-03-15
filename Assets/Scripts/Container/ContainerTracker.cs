using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Container
{
    public class ContainerTracker
    {
        private List<ContainerTrackerInformation> _containerTrackerInformation;

        public ContainerTracker()
        {
            _containerTrackerInformation = new List<ContainerTrackerInformation>();
        }

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

        public void ClearContainer(ContainerInstance container = null)
        {
            if (container == null)
            {
                _containerTrackerInformation.Clear();
                return;
            }

            _containerTrackerInformation.RemoveAll(
                c => c.ContainerInstance.GetInstanceID() == container.GetInstanceID());
        }
        
        private ContainerInstance GetContainerFromPlayer(int playerIndex) =>
            _containerTrackerInformation.First(c => c.ContainsPlayerIndex(playerIndex)).ContainerInstance;

        private List<int> GetPlayersFromContainer(ContainerInstance container) => _containerTrackerInformation
            .First(c => c.ContainerInstance == container).PlayerIndexes;

        private bool IsContainerRegistered(ContainerInstance container) =>
            GetInformationFromContainer(container) != null;

        private ContainerTrackerInformation GetInformationFromContainer(ContainerInstance container) =>
            _containerTrackerInformation.First(c => c.ContainerInstance.GetInstanceID() == container.GetInstanceID());
    }

    public class ContainerTrackerInformation
    {
        public ContainerInstance ContainerInstance { get; }
        public List<int> PlayerIndexes { get; }

        public ContainerTrackerInformation(ContainerInstance containerInstance, int playerIndex = -1,
            List<int> playerIndexes = null)
        {
            if (playerIndex != -1 && playerIndexes != null)
            {
                throw new ArgumentException("Only one of playerIndex or playerIndexes can be specified.");
            }

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