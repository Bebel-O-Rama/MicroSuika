using System;
using MultiSuika.Manager;
using MultiSuika.Utilities;
using TMPro;
using UnityEngine;

namespace MultiSuika.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] public int playerIndex;
        [SerializeField] public TMP_Text tmp;
        [SerializeField] public Color connectedColor;
        [SerializeField] public Color disconnectedColor;

        private bool _isActive;
        private FloatReference _playerScore;

        private void Awake()
        {
            _isActive = false;
            tmp.color = disconnectedColor;
        }


        private void Update()
        {
            if (!_isActive)
                return;

            tmp.text = string.Format($"{_playerScore.Value:0}");
        }

        public void SetScoreboardActive(bool isActive)
        {
            if (_isActive == isActive)
                return;
            if (isActive)
            {
                _playerScore = ScoreHandler.Instance.GetPlayerScoreReference(playerIndex);
                tmp.color = connectedColor;
            }
            else
            {
                _playerScore = null;
                tmp.color = disconnectedColor;
                tmp.text = "0";
            }

            _isActive = isActive;
        }
    }
}