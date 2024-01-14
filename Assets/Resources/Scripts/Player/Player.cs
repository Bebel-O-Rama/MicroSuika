using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // private bool areInputsActive = false;

    [SerializeField] private Cannon _mainCannon;
    [SerializeField] private Cannon _miniGameCannon;
    [SerializeField] public PlayerInputHandler playerInputHandler;
    
    private int _playerIndex;

    public void InitializePlayer(PlayerData playerData, GameModeData gameModeData = null, int numberPlayerConnected = 0)
    {
        _mainCannon.SetScoreReference(playerData.mainScore);
        _miniGameCannon.SetScoreReference(playerData.miniGameScore);
        _playerIndex = playerData.playerIndexNumber;

        if (gameModeData != null)
        {
            UpdateAndSwitchCannon(gameModeData, numberPlayerConnected);
        }
    }

    public void UpdateAndSwitchCannon(GameModeData gameModeData, int numberPlayerConnected = 0)
    {
        var cannonToUse = gameModeData.isMainCannon ? _mainCannon : _miniGameCannon;
        DeactivateCannons();
        UpdateCannonParameters(gameModeData, cannonToUse, numberPlayerConnected);
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

    private void UpdateCannonParameters(GameModeData gameModeData, Cannon cannon, int numberPlayerConnected = 0)
    {
        // TODO : Cleanup how we fetch the initial position list for the cannon (it's a bit sketchy right now)
        var cannonSpawnSetData = gameModeData.cannonSpawnPositionSetData.GetCannonSpawnPositionData(numberPlayerConnected);

        // var cannonPositionData = gameModeData.cannonSpawnPositionSetData[_playerIndex];
        Vector2 centerPosition = cannonSpawnSetData[_playerIndex].centerPosition;
        Vector2 spawnPosition = centerPosition;
        Vector2 horizontalMargin = centerPosition;
        horizontalMargin.x -= cannonSpawnSetData[_playerIndex].maxHorizontalDelta;
        horizontalMargin.y += cannonSpawnSetData[_playerIndex].maxHorizontalDelta;
        
        spawnPosition.x = Random.Range(centerPosition.x - cannonSpawnSetData[_playerIndex].xRandomSpawnRangeDelta, centerPosition.x + cannonSpawnSetData[_playerIndex].xRandomSpawnRangeDelta);
        
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
