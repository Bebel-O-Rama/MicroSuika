using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private GameObject spriteObj;
    
    // Cannon Parameters
    private float _speed;
    private float _reloadCooldown;
    private float _shootingForce;
    
    private bool _isUsingPeggleMode = false;
    private bool _isCannonActive = false;
    
    // Positioning
    private Vector3 _centeredPosition;
    private Vector2 _horizontalMargin;
    private float _shootingAngle = 0f;
    private Vector2 _shootingDirection = Vector2.down;

    // Ball Parameters
    private BallSetData _ballSetData;
    private Ball _currentBall;
    private IntReference _scoreReference;
    
    public void UpdateParameters(CannonData cannonData, Vector2 centerPosition, Vector2 spawnPosition , Vector2 horizontalMargin, BallSetData ballSetData)
    {
        _speed = cannonData.speed.Value;
        _reloadCooldown = cannonData.reloadCooldown.Value;
        _shootingForce = cannonData.shootingForce.Value;
        _isUsingPeggleMode = cannonData.isUsingPeggleMode.Value;

        _centeredPosition = centerPosition;
        _horizontalMargin = horizontalMargin;
        _shootingAngle = 0f;
        _shootingDirection = Vector2.down;
        transform.position = spawnPosition;

        _ballSetData = ballSetData;
        // TEMPORARY, It's just to better see when a cannon is active of not
        spriteObj.SetActive(true);
    }

    public bool IsCannonActive() => _isCannonActive;

    public void DestroyCurrentBall()
    {
        if (_currentBall != null)
            Destroy(_currentBall.gameObject);
    }

    public void SetScoreReference(IntReference scoreRef) => _scoreReference = scoreRef;

    public void SetCannonControlConnexion(PlayerInputHandler playerInputHandler, bool isActive)
    {
        if (isActive)
        {
            playerInputHandler.OnHorizontalMvtContinuous += MoveCannon;
            playerInputHandler.OnShoot += DropBall;
            _isCannonActive = true;
            if (_currentBall == null)
                LoadNewBall();

        }
        else
        {
            playerInputHandler.OnHorizontalMvtContinuous -= MoveCannon;
            playerInputHandler.OnShoot -= DropBall;
            _isCannonActive = false;
            // TEMPORARY, It's just to better see when a cannon is active of not
            spriteObj.SetActive(false);
        }
    }
    
    public void DropBall()
    {
        if (_currentBall == null)
            return;
        
        _currentBall.EnableCollision();
        _currentBall.rb2d.AddForce(_shootingDirection * _shootingForce);
        _currentBall = null;
        Invoke("LoadNewBall", _reloadCooldown);
    }
    
    public void MoveCannon(float xAxis)
    {
       if (_isUsingPeggleMode)
        {
            if (xAxis < 0 && _shootingAngle > -Mathf.PI / 2 + 0.1f || xAxis > 0 && _shootingAngle < Mathf.PI / 2 - 0.1f)
            {
                _shootingAngle += xAxis * _speed * Time.deltaTime;
                _shootingDirection = new Vector2(Mathf.Sin(_shootingAngle), -Mathf.Cos(_shootingAngle));
            }
        }
        else
        {
            if (xAxis < 0 && transform.position.x > _horizontalMargin.x || xAxis > 0 && transform.position.x < _horizontalMargin.y)
                transform.Translate(xAxis*Time.deltaTime*_speed, 0, 0);
        }

        if (_currentBall != null)
            _currentBall.transform.position = (Vector2)transform.position + _shootingDirection;
    }
    
    private void LoadNewBall()
    {
        _currentBall = _ballSetData.SpawnNewBall((Vector2)transform.position + _shootingDirection, _scoreReference, disableCollision: true);
    }
    
}