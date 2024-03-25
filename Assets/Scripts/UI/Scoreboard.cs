using MultiSuika.Ball;
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
        private int _playerScore;

        private void Awake()
        {
            _isActive = false;
            tmp.color = disconnectedColor;
            _playerScore = 0;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            tmp.text = string.Format($"{_playerScore:0}");
        }

        public void SetScoreboardActive(bool isActive)
        {
            if (_isActive == isActive)
                return;
            if (isActive)
            {
                BallTracker.Instance.OnBallFusion.Subscribe(OnBallFusion, playerIndex);
                tmp.color = connectedColor;
            }
            else
            {
                BallTracker.Instance.OnBallFusion.Unsubscribe(OnBallFusion, playerIndex);
                _playerScore = 0;
                tmp.color = disconnectedColor;
                tmp.text = "0";
            }

            _isActive = isActive;
        }
        
        private void OnBallFusion(BallInstance ball)
        {
            _playerScore += ball.ScoreValue;
        }
    }
}