using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Skin
{
    [CreateAssetMenu(menuName = "Skin/Complete Theme Data")]
    public class SkinData : ScriptableObject
    {
        [SerializeField] private List<PlayerVariationSkinData> _playersSkinData;

        public PlayerVariationSkinData GetPlayerSkinData(int playerNumber) => _playersSkinData.Count >= playerNumber + 1
            ? _playersSkinData[playerNumber]
            : _playersSkinData[0];
    }
}
