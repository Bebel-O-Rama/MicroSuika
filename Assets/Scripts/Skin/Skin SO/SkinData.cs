using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.Skin
{
    [CreateAssetMenu(menuName = "Skin/Complete Skin Data")]
    public class SkinData : ScriptableObject
    {
        public List<PlayerSkinData> playersSkinData;

        public PlayerSkinData GetPlayerSkinData(int playerNumber) => playersSkinData.Count >= playerNumber + 1
            ? playersSkinData[playerNumber]
            : playersSkinData[0];
    }
}
