using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Audio
{
    [CreateAssetMenu(menuName = "Audio/Wwise Events")]
    public class WwiseEventsData : ScriptableObject
    {
        [SerializeField] private List<AK.Wwise.Event> _wwiseEvents;

        /// <summary>
        ///     Find a WwiseEvent using the index and post it on a GameObject
        /// </summary>
        /// <param name="index">The index in the WwiseEventsData list</param>
        /// <param name="gameObject">The GameObject on which to post the event</param>
        /// <returns>Returns true if successful</returns>
        public bool PostEventAtIndex(int index, GameObject gameObject)
        {
            if (gameObject == null || index > _wwiseEvents.Count - 1)
                return false;
            var ret = _wwiseEvents[index].Post(gameObject);
            return ret != AkSoundEngine.AK_INVALID_PLAYING_ID;
        }
    }
}
