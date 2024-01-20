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
    
    public bool isUsingPeggleMode = false;
    public bool isCannonActive = false;
    
    // Positioning
    public float horizontalMargin;
    private float _shootingAngle = 0f;
    private Vector2 _shootingDirection = Vector2.down;

    // Ball Parameters
    private Ball _currentBall;
    public BallSetData ballSetData;
    public IntReference scoreReference;
    
    public bool IsCannonActive() => isCannonActive;

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
            isCannonActive = true;
            if (_currentBall == null)
                LoadNewBall();

        }
        else
        {
            playerInputHandler.OnHorizontalMvtContinuous -= MoveCannon;
            playerInputHandler.OnShoot -= DropBall;
            isCannonActive = false;
        }
    }
    
    public void DropBall()
    {
        if (_currentBall == null)
            return;
        
        _currentBall.EnableCollision();
        _currentBall.rb2d.AddForce(_shootingDirection * shootingForce);
        _currentBall = null;
        Invoke("LoadNewBall", reloadCooldown);
    }
    
    public void MoveCannon(float xAxis)
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
            _currentBall.transform.localPosition = (Vector2)transform.localPosition + _shootingDirection;
    }
    
    private void LoadNewBall()
    {
        _currentBall = ballSetData.SpawnNewBall((Vector2)transform.localPosition + _shootingDirection, scoreReference, disableCollision: true);
    }
    
}