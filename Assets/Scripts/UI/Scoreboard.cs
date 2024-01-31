using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] public TMP_Text tmp;
    [SerializeField] public IntVariable playerScore;
    [SerializeField] public Color connectedColor;
    [SerializeField] public Color disconnectedColor;
    
    private bool isConnected;
    
    private void Start()
    {
        isConnected = playerScore != null;
        tmp.color = isConnected ? connectedColor : disconnectedColor;
    }

    private void Update()
    {
        if (playerScore != null)
        {
            if (!isConnected)
            {
                tmp.color = connectedColor;
                isConnected = true;
            }
            tmp.text = string.Format($"{playerScore.Value}");
            return;
        }

        if (isConnected)
        {
            tmp.text = "0";
            tmp.color = disconnectedColor;
            isConnected = false;
        }
    }
}
