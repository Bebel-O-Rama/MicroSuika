using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiSuika.Cannon
{
    public class CannonTracker
    {
        private readonly List<CannonTrackerInformation>
            _cannonTrackerInformation = new List<CannonTrackerInformation>();
        
        public void AddNewCannon(CannonInstance cannon, int playerIndex)
        {
            if (IsCannonRegistered(cannon))
                return;
            _cannonTrackerInformation.Add(new CannonTrackerInformation(cannon, playerIndex));
        }

        public void ClearCannons()
        {
            for (int i = _cannonTrackerInformation.Count - 1; i >= 0; i--)
                ClearCannon(_cannonTrackerInformation[i].CannonInstance);
        }
        public void ClearCannon(CannonInstance cannon)
        {
            var info = _cannonTrackerInformation.FirstOrDefault(info =>
                info.CannonInstance.GetInstanceID() == cannon.GetInstanceID());
            
            cannon.DestroyCurrentBall();
            Object.Destroy(cannon.gameObject);
            
            if (info != null)
                _cannonTrackerInformation.Remove(info);
        }

        public List<CannonInstance> GetCannons() =>
            _cannonTrackerInformation.Select(info => info.CannonInstance).ToList();

        private CannonInstance GetCannonFromPlayer(int playerIndex) =>
            _cannonTrackerInformation.FirstOrDefault(c => c.PlayerIndex == playerIndex)?.CannonInstance;

        private int GetPlayerFromCannon(CannonInstance cannon) => _cannonTrackerInformation
            .FirstOrDefault(c => c.CannonInstance == cannon)?.PlayerIndex ?? -1;

        private bool IsCannonRegistered(CannonInstance cannon) => GetInformationFromCannon(cannon) != null;

        private CannonTrackerInformation GetInformationFromCannon(CannonInstance cannon) =>
            _cannonTrackerInformation.FirstOrDefault(c => c.CannonInstance.GetInstanceID() == cannon.GetInstanceID());
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