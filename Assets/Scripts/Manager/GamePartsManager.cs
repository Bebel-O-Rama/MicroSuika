using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MultiSuika.Container;
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

        private ContainerInformation _containerInformation;

        // TODO: Check if that's necessary
        public void Initialize()
        {
            _containerInformation = new ContainerInformation();
        }
        
        



    }


    public class ContainerInformation
    {
        private List<ContainerInstance> _containerInstances;
        private Dictionary<int, ContainerInstance> _containerPerPlayer;

        // TODO: Check if that's necessary
        public ContainerInformation()
        {
            _containerInstances = new List<ContainerInstance>();
            _containerPerPlayer = new Dictionary<int, ContainerInstance>();
        }
        
        public void AddNewContainer(ContainerInstance container)
        {
            if (!CheckIfContainerRegistered(container))
                _containerInstances.Add(container);
        }

        public void ClearContainer(ContainerInstance container)
        {
            _containerInstances.Remove(container);
            foreach (var item in _containerPerPlayer.Where(kvp => kvp.Value == container).ToList())
            {
                _containerPerPlayer.Remove(item.Key);
            }
        }

        public void ConnectPlayerToContainer(int playerIndex, ContainerInstance container)
        {
            if (!CheckIfContainerRegistered(container))
            {
                AddNewContainer(container);
            }

            if (_containerPerPlayer.ContainsKey(playerIndex))
                return; // Something weird is happening
            _containerPerPlayer.Add(playerIndex, container);
        }

        public ContainerInstance GetContainerFromPlayerIndex(int playerIndex) => _containerPerPlayer[playerIndex];

        public List<int> GetPlayerIndexFromContainer(ContainerInstance container) => _containerPerPlayer
            .Where(kvp => kvp.Value == container).Select(kvp => kvp.Key).ToList();

        private bool CheckIfContainerRegistered(ContainerInstance container) =>
            _containerInstances.Find(c => c.GetInstanceID() == container.GetInstanceID());
    }
}