using TMPro;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public class WatermarkPicker : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            foreach (var tmpText in FindObjectsOfType<TMP_Text>())
            {
                if (tmpText.text == "trial version")
                    Destroy(tmpText);
            }
        }
    }
}
