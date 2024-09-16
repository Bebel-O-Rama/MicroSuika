using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Nova;
using UnityEngine;

namespace MultiSuika.UI
{
    public class PlayerScoreStrip : MonoBehaviour
    {
        [SerializeField] public UIBlock2D playerIcon;
        [SerializeField] public List<UIBlock2D> scoreIcons;
        
        [SerializeField] private Color _dimmedScoreIconColor;
        [SerializeField] private Color _highlightScoreIconColor;

        // Ish I know, I'll refactor that once everything is working
        public int playerIndex;

        // public void StartScoreStripSequence()
        // {
        //     var scoreStripSequence = DOTween.Sequence();
        //     scoreStripSequence
        //         .Append(DOTween.To(() => _playerIcon.Color, c => _playerIcon.Color = c,
        //             Color.white, 1));
        // }
        
        
        public void SetScoreStripParameters(int playerIndex, Sprite playerIcon)
        {
            this.playerIcon.SetImage(playerIcon);
            this.playerIndex = playerIndex;
        }
    }
}
