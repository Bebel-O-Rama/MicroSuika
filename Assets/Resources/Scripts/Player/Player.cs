using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Cannon _mainCannon;
    [SerializeField] private Cannon _miniGameCannon;
    [SerializeField] public PlayerInputHandler playerInputHandler;
    
    private int _playerIndex;

    public void InitializePlayer(PlayerData playerData, GameModeData gameModeData = null)
    {
        _mainCannon.SetScoreReference(playerData.mainScore);
        _miniGameCannon.SetScoreReference(playerData.miniGameScore);
        _playerIndex = playerData.playerIndexNumber;

        if (gameModeData != null)
        {
            UpdateAndSwitchCannon(gameModeData);
        }
    }

    public void UpdateAndSwitchCannon(GameModeData gameModeData)
    {
        var cannonToUse = gameModeData.isMainCannon ? _mainCannon : _miniGameCannon;
        DeactivateCannons();
        UpdateCannonParameters(gameModeData, cannonToUse);
    }

    public void DestroyPlayerCurrentBall()
    {
        _mainCannon.DestroyCurrentBall();
        _miniGameCannon.DestroyCurrentBall();
    }
    
    // TODO : Refactor this once we have actual sprite and skins for the cannon  
    public void UpdateMainCannonColor(Color color) => _mainCannon.GetComponentInChildren<SpriteRenderer>().color = color;
    // TODO : Refactor this once we have actual sprite and skins for the cannon  
    public Cannon GetCannon(bool isMainCannon) => isMainCannon ? _mainCannon : _miniGameCannon;

    private void UpdateCannonParameters(GameModeData gameModeData, Cannon cannon)
    {
        var cannonPositionData = gameModeData.cannonInitializationData[_playerIndex];
        Vector2 centerPosition = cannonPositionData.centerPosition;
        Vector2 spawnPosition = centerPosition;
        Vector2 horizontalMargin = centerPosition;
        horizontalMargin.x -= cannonPositionData.maxHorizontalDelta;
        horizontalMargin.y += cannonPositionData.maxHorizontalDelta;
        
        spawnPosition.x = Random.Range(centerPosition.x - cannonPositionData.xRandomSpawnRangeDelta, centerPosition.x + cannonPositionData.xRandomSpawnRangeDelta);
        
        cannon.UpdateParameters(gameModeData.cannonData, centerPosition, spawnPosition, horizontalMargin, gameModeData.ballSetData);
        StartCoroutine(ActivateCannon(cannon, gameModeData.cooldownBeforeInputConnexion));
    }

    private IEnumerator ActivateCannon(Cannon cannon, float delayBeforeActivation)
    {
        yield return new WaitForSeconds(delayBeforeActivation);
        cannon.SetCannonControlConnexion(playerInputHandler, true);
    }
    
    private void DeactivateCannons()
    {
        if (_mainCannon.IsCannonActive())
            _mainCannon.SetCannonControlConnexion(playerInputHandler, false);
        if (_miniGameCannon)
            _miniGameCannon.SetCannonControlConnexion(playerInputHandler, false);
    }
}
