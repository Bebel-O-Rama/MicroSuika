using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiSuika.GameLogic
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera defaultCamera;
        [SerializeField] private List<Camera> playerCameras;

        public Camera GetPlayerCamera(int playerIndex) => playerCameras[playerIndex];
        public Camera GetPlayerCameraWithTag(int playerTag)
        {
            return LayerMask.LayerToName(playerTag) switch
            {
                "Player1" => playerCameras[0],
                "Player2" => playerCameras[1],
                "Player3" => playerCameras[2],
                "Player4" => playerCameras[3],
                _ => playerCameras[0]
            };
        }
    }
}
