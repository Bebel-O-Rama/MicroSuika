using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiSuika.Cannon
{
    public class CannonTracker : MonoBehaviour
    {
        private List<CannonTrackerInformation> _cannonTrackerInformation;

        public CannonTracker()
        {
            _cannonTrackerInformation = new List<CannonTrackerInformation>();
        }

        public void AddNewCannon(CannonInstance cannon, int playerIndex)
        {
            if (IsCannonRegistered(cannon))
                return;
            _cannonTrackerInformation.Add(new CannonTrackerInformation(cannon, playerIndex));
        }

        public void ClearCannon(CannonInstance cannon = null)
        {
            if (cannon == null)
            {
                _cannonTrackerInformation.Clear();
                return;
            }

            _cannonTrackerInformation.RemoveAll(
                c => c.CannonInstance.GetInstanceID() == cannon.GetInstanceID());
        }

        private CannonInstance GetCannonFromPlayer(int playerIndex) =>
            _cannonTrackerInformation.First(c => c.PlayerIndex == playerIndex).CannonInstance;

        private int GetPlayerFromCannon(CannonInstance cannon) => _cannonTrackerInformation
            .First(c => c.CannonInstance == cannon).PlayerIndex;

        private bool IsCannonRegistered(CannonInstance cannon) => GetInformationFromCannon(cannon) != null;

        private CannonTrackerInformation GetInformationFromCannon(CannonInstance cannon) =>
            _cannonTrackerInformation.First(c => c.CannonInstance.GetInstanceID() == cannon.GetInstanceID());
    }

    public class CannonTrackerInformation
    {
        public CannonInstance CannonInstance { get; }
        public int PlayerIndex { get; }

        public CannonTrackerInformation(CannonInstance cannonInstance, int playerIndex)
        {
            CannonInstance = cannonInstance;
            PlayerIndex = playerIndex;
        }
    }
}