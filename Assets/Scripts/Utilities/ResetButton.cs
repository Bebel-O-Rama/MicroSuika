using MultiSuika.Manager;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public class ResetButton : MonoBehaviour
    {
        [SerializeField] private KeyCode key;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(key))
                VersusManager.Instance.ResetGame();
        
        }
    }
}
