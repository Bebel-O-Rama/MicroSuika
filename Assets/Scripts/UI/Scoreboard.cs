using MultiSuika.Utilities;
using TMPro;
using UnityEngine;

namespace MultiSuika.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] public TMP_Text tmp;
        [SerializeField] public IntVariable playerScore;
        [SerializeField] public Color connectedColor;
        [SerializeField] public Color disconnectedColor;
    
        private bool _isConnected;
    
        private void Start()
        {
            _isConnected = playerScore != null;
            tmp.color = _isConnected ? connectedColor : disconnectedColor;
        }

        private void Update()
        {
            if (playerScore != null)
            {
                if (!_isConnected)
                {
                    tmp.color = connectedColor;
                    _isConnected = true;
                }
                tmp.text = string.Format($"{playerScore.Value}");
                return;
            }

            if (_isConnected)
            {
                tmp.text = "0";
                tmp.color = disconnectedColor;
                _isConnected = false;
            }
        }
    }
}
