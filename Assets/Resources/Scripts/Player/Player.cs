using UnityEngine;

public class Player : MonoBehaviour
{
    public Cannon cannon;
    public IntReference score;
    public int playerIndex;
    [SerializeField] 
    public PlayerInputHandler playerInputHandler;
    
    // private int _playerIndex;

    // public void InitializePlayer(PlayerData playerData, GameModeData gameModeData = null)
    // {
    //     _playerIndex = playerData.playerIndexNumber;
    //     
    // }
    
    public void DestroyPlayerCurrentBall()
    {
        cannon.DestroyCurrentBall();
    }
    
    // TODO : Refactor this once we have actual sprite and skins for the cannon  

    // public void UpdateMainCannonColor(Color color) => cannon.GetComponentInChildren<SpriteRenderer>().color = color;
    // TODO : Refactor this once we have actual sprite and skins for the cannon  
    // public Cannon GetCannon() => cannon;
    

    // private IEnumerator ActivateCannon(Cannon cannon, float delayBeforeActivation)
    // {
    //     yield return new WaitForSeconds(delayBeforeActivation);
    //     cannon.SetCannonControlConnexion(playerInputHandler, true);
    // }
    //
    // private void DeactivateCannons()
    // {
    //     if (cannon.IsCannonActive())
    //         cannon.SetCannonControlConnexion(playerInputHandler, false);
    // }
}
