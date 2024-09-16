using System;
using System.Collections.Generic;
using DG.Tweening;
using MultiSuika.Manager;
using Nova;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiSuika.UI
{
    public class ScoreTransitionVersus : MonoBehaviour
    {
        [SerializeField] private UIBlock2D _scoreTransitionMain;
        [SerializeField] private PlayerScoreStrip _playerScoreStripObject;

        [SerializeField] private List<float> _spacingBetweenPlayers;
        [SerializeField] private List<Sprite> _playerMainIcons;

        [SerializeField] private UIBlock2D _mainScoreUIblock;
        [SerializeField] private List<SpriteRenderer> _planetCycleBanners;

        // [SerializeField] private Color _blackboardColorHidden;
        [FormerlySerializedAs("_blackboardFadeInDelay")]
        [Header("Transition delay")] 
        [SerializeField] private float _blackboardFadeDelay;
        
        [Header("Transition Colors")]
        [SerializeField] private Color _blackboardColorVisible;
        [SerializeField] private Color _dimmedPlanetCycleColor;
        [SerializeField] private Color _dimmedScoreIconColor;
        [SerializeField] private Color _highlightScoreIconColor;

        private Color _hiddenColor = new Color(0, 0, 0, 0);

        private List<PlayerScoreStrip> _playerScoreStrips = new List<PlayerScoreStrip>();

        private void Start()
        {
            ResetScoreStrips();
            // StartScoreTransitionSequence(1);
        }

        public void ScoreTransitionSequence(int winnerPlayerIndex, bool isTargetTriggered)
        {
            _mainScoreUIblock.gameObject.SetActive(true);
            var scoreTransitionSequence = DOTween.Sequence();
            // scoreTransitionSequence.AppendInterval(2);

            scoreTransitionSequence.AppendCallback(TransitionStart);
            // scoreTransitionSequence.AppendInterval(5);
            // scoreTransitionSequence.AppendCallback(TransitionEnd);
            
            // // Blackboard fade-in
            // scoreTransitionSequence
            //     .Append(DOTween.To(() => _mainScoreUIblock.Color, c => _mainScoreUIblock.Color = c,
            //         _blackboardColorVisible, _blackboardFadeDelay));
            //
            //
            // foreach (var cycleBanner in _planetCycleBanners)
            // {
            //     scoreTransitionSequence.Join(cycleBanner.DOColor(_dimmedPlanetCycleColor, _blackboardFadeDelay));
            // }
            
            
            
            
            
            
            // scoreTransitionSequence.AppendInterval(0.1f);
            
            // // Player icons and old score icons fade-in
            // var playerScores = ScoreManager.Instance.GetPlayerScores();
            // foreach (var scoreStrip in _playerScoreStrips)
            // {
            //     scoreTransitionSequence.Join(DOTween.To(() => scoreStrip.playerIcon.Color,
            //         c => scoreStrip.playerIcon.Color = c,
            //         Color.white, 1));
            //
            //     var playerScore = playerScores[scoreStrip.playerIndex];
            //     var highlightedScoreIcons = scoreStrip.scoreIcons.GetRange(0, playerScore);
            //     var dimmedScoreIcons = scoreStrip.scoreIcons.GetRange(playerScore, scoreStrip.scoreIcons.Count - playerScore);
            //
            //     foreach (var highlightedIcon in highlightedScoreIcons)
            //     {
            //         scoreTransitionSequence.Join(DOTween.To(() => highlightedIcon.Color,
            //             c => highlightedIcon.Color = c,
            //             _highlightScoreIconColor, 1));
            //     }
            //
            //     foreach (var dimmedIcon in dimmedScoreIcons)
            //     {
            //         scoreTransitionSequence.Join(DOTween.To(() => dimmedIcon.Color,
            //             c => dimmedIcon.Color = c,
            //             _dimmedScoreIconColor, 1));
            //     }
            // }


            // TransitionScoreStart();
            //
            //
            // scoreTransitionSequence.AppendInterval(0.1f);
            // scoreTransitionSequence.AppendInterval(5f);
            // scoreTransitionSequence.AppendInterval(0.1f);


            // foreach (var scoreStrip in _playerScoreStrips)
            // {
            //     scoreTransitionSequence.Join(DOTween.To(() => scoreStrip.playerIcon.Color,
            //         c => scoreStrip.playerIcon.Color = c,
            //         _hiddenColor, 2));
            //     foreach (var scoreIcon in scoreStrip.scoreIcons)
            //     {
            //         scoreTransitionSequence.Join(DOTween.To(() => scoreIcon.Color,
            //             c => scoreIcon.Color = c,
            //             _hiddenColor, 2));
            //     }
            // }


            // TransitionScoreEnd();
            
            
            // scoreTransitionSequence.AppendInterval(0.1f);
            
            // scoreTransitionSequence
            //     .Join(DOTween.To(() => _mainScoreUIblock.Color, c => _mainScoreUIblock.Color = c,
            //         _hiddenColor, _blackboardFadeDelay));
            //
            // foreach (var cycleBanner in _planetCycleBanners)
            // {
            //     scoreTransitionSequence.Join(cycleBanner.DOColor(Color.white, _blackboardFadeDelay));
            // }
            
            
            // TransitionBlackboardEnd();

            
            // scoreTransitionSequence.Play();



            if (isTargetTriggered)
            {
                Debug.Log($"Player {winnerPlayerIndex + 1} is the winneeeer!");
                Debug.Log("**********************************************************");
                ScoreManager.Instance.ResetScores();
            }
        }

        private void TransitionStart()
        {
            var seq = DOTween.Sequence();

            seq.AppendInterval(2);
            seq.AppendCallback(TransitionBlackboardStart);
            seq.AppendCallback(TransitionScoreStart);

            seq.AppendInterval(5);

            seq.OnComplete(TransitionEnd);

            // return seq.Complete;
            // TransitionBlackboardStart();
        }

        private void TransitionEnd()
        {
            var seq = DOTween.Sequence();

            seq.AppendCallback(TransitionBlackboardEnd);
            seq.AppendInterval(1);
            seq.AppendCallback(TransitionScoreEnd);
        }
        
        private void TransitionBlackboardStart()
        {
            DOTween.To(() => _mainScoreUIblock.Color, c => _mainScoreUIblock.Color = c,
                    _blackboardColorVisible, _blackboardFadeDelay);
            foreach (var cycleBanner in _planetCycleBanners)
            {
                cycleBanner.DOColor(_dimmedPlanetCycleColor, _blackboardFadeDelay);
            }
        }

        private void TransitionBlackboardEnd()
        {
            DOTween.To(() => _mainScoreUIblock.Color, c => _mainScoreUIblock.Color = c,
                    _hiddenColor, _blackboardFadeDelay);
            foreach (var cycleBanner in _planetCycleBanners)
            {
                cycleBanner.DOColor(Color.white, _blackboardFadeDelay);
            }
        }

        private void TransitionScoreStart()
        {
            var playerScores = ScoreManager.Instance.GetPlayerScores();
            foreach (var scoreStrip in _playerScoreStrips)
            {
                DOTween.To(() => scoreStrip.playerIcon.Color,
                    c => scoreStrip.playerIcon.Color = c,
                    Color.white, 1);

                var playerScore = playerScores[scoreStrip.playerIndex];
                var highlightedScoreIcons = scoreStrip.scoreIcons.GetRange(0, playerScore);
                var dimmedScoreIcons = scoreStrip.scoreIcons.GetRange(playerScore, scoreStrip.scoreIcons.Count - playerScore);

                foreach (var highlightedIcon in highlightedScoreIcons)
                {
                    DOTween.To(() => highlightedIcon.Color,
                        c => highlightedIcon.Color = c,
                        _highlightScoreIconColor, 1);
                }

                foreach (var dimmedIcon in dimmedScoreIcons)
                {
                    DOTween.To(() => dimmedIcon.Color,
                        c => dimmedIcon.Color = c,
                        _dimmedScoreIconColor, 1);
                }
            }
        }

        private void TransitionScoreEnd()
        {
            foreach (var scoreStrip in _playerScoreStrips)
            {
                DOTween.To(() => scoreStrip.playerIcon.Color,
                    c => scoreStrip.playerIcon.Color = c,
                    _hiddenColor, 2);
                foreach (var scoreIcon in scoreStrip.scoreIcons)
                {
                    DOTween.To(() => scoreIcon.Color,
                        c => scoreIcon.Color = c,
                        _hiddenColor, 2);
                }
            }
        }

        private void ResetScoreStrips()
        {
            foreach (var scoreStrip in _playerScoreStrips)
            {
                Destroy(scoreStrip.gameObject);
            }
            _playerScoreStrips.Clear();
            
            int playerNb = PlayerManager.Instance.GetNumberOfActivePlayer();
            for (int i = 0; i < playerNb; i++)
            {
                var newScoreStrip = Instantiate(_playerScoreStripObject, _mainScoreUIblock.transform);
                newScoreStrip.SetScoreStripParameters(i, _playerMainIcons[i]);
                _playerScoreStrips.Add(newScoreStrip);
                // newScoreStrip.playerIcon.Color = _hiddenColor;
            }
            
            _scoreTransitionMain.AutoLayout.Spacing.Value = _spacingBetweenPlayers[playerNb - 1];

        }
    }
}
