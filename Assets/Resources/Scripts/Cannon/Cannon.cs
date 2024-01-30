using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Cannon : MonoBehaviour
{
    [SerializeField] public GameObject spriteObj;
    
    // Cannon Parameters
    public float speed;
    public float reloadCooldown;
    public float shootingForce;
    public float emptyDistanceBetweenBallAndCannon;
    public bool isUsingPeggleMode = false;
    
    // Positioning
    public float horizontalMargin;
    private float _shootingAngle = 0f;
    private Vector2 _shootingDirection = Vector2.down;

    // Ball Parameters
    public BallSetData ballSetData;
    public IntReference scoreReference;
    public Container container;
    private Ball _currentBall;
    private float _currentBallDistanceFromCannon;

    //Wwise Event
    public AK.Wwise.Event WwiseEventCannonShoot;



    public void DestroyCurrentBall()
    {
        if (_currentBall != null)
            Destroy(_currentBall.gameObject);
    }
    
    public void SetCannonControlConnexion(PlayerInputHandler playerInputHandler, bool isActive)
    {
        if (isActive)
        {
            playerInputHandler.OnHorizontalMvtContinuous += MoveCannon;
            playerInputHandler.OnShoot += DropBall;
            if (_currentBall == null)
                LoadNewBall();
        }
        else
        {
            playerInputHandler.OnHorizontalMvtContinuous -= MoveCannon;
            playerInputHandler.OnShoot -= DropBall;
        }
    }
    
    private void DropBall()
    {
        if (_currentBall == null)
            return;
        
        _currentBall.EnableCollision();
        _currentBall.rb2d.AddForce(_shootingDirection.normalized * shootingForce);
        _currentBall = null;
        Invoke("LoadNewBall", reloadCooldown);
        Debug.Log("Drop");
        WwiseEventCannonShoot.Post(gameObject);
    }
    
    private void MoveCannon(float xAxis)
    {
       if (isUsingPeggleMode)
       {
           if (xAxis < 0 && _shootingAngle > -Mathf.PI / 2 + 0.1f || xAxis > 0 && _shootingAngle < Mathf.PI / 2 - 0.1f)
           {
               _shootingAngle += xAxis * speed * Time.deltaTime;
               _shootingDirection = new Vector2(Mathf.Sin(_shootingAngle), -Mathf.Cos(_shootingAngle));
           }
       }
       else
       {
           if (xAxis < 0 && transform.localPosition.x > -horizontalMargin || xAxis > 0 && transform.localPosition.x < horizontalMargin)
               transform.Translate(xAxis*Time.deltaTime*speed, 0, 0);
       }

       if (_currentBall != null)
           _currentBall.transform.localPosition = (Vector2)transform.localPosition + _shootingDirection.normalized * _currentBallDistanceFromCannon;
    }
    
    private void LoadNewBall()
    {
        int newBallIndex = ballSetData.GetRandomBallTier();
        _currentBallDistanceFromCannon = ballSetData.GetBallData(newBallIndex).scale / 2f + emptyDistanceBetweenBallAndCannon;
        _currentBall = Initializer.InstantiateBall(ballSetData, container,
            (Vector2)transform.localPosition + _shootingDirection.normalized * _currentBallDistanceFromCannon);
        Initializer.SetBallParameters(_currentBall, newBallIndex, scoreReference, ballSetData, container, true);
    }
    
}