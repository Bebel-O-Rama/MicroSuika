using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Cannon Basic Parameters")]
    [Min(0f)] [SerializeField] public float speed;
    [Min(0f)] [SerializeField] public float reloadCooldown;
    [Min(0f)] [SerializeField] public float boundariesFromCenter;
    [Min(0f)] [SerializeField] public float shootingForce = 200f;
    [Range(-Mathf.PI/2+0.1f, Mathf.PI/2-0.1f)] [SerializeField] public float shootingAngle = 0f;
    [SerializeField] public Vector2 shootingDirection = Vector2.down;
    
    [Header("General Game Parameters")]
    [SerializeField] public PlayerGameData playerGameData;
    [SerializeField] public IntReference playerScore;

    [Header("Cannon Modifiers")]
    [SerializeField] public bool isDisabled = false;
    [SerializeField] public bool isUsingPeggleMode = false;
    
    private Vector3 _centeredPosition;
    private Vector2 _horizontalMargin;
    private Ball _currentBall;
    
    private void OnEnable()
    {
        InitializePlayerData();
        LoadNewBall();
    }

    public void DropBall()
    {
        if (isDisabled || _currentBall == null)
            return;
        
        _currentBall.EnableCollision();
        _currentBall.rb2d.AddForce(shootingDirection * shootingForce);
        _currentBall = null;
        Invoke("LoadNewBall", reloadCooldown);
    }
    
    public void MoveCannon(float xAxis)
    {
        if (isDisabled)
            return;
        
        if (isUsingPeggleMode)
        {
            if (xAxis < 0 && shootingAngle > -Mathf.PI / 2 + 0.1f || xAxis > 0 && shootingAngle < Mathf.PI / 2 - 0.1f)
            {
                shootingAngle += xAxis * speed * Time.deltaTime;
                shootingDirection = new Vector2(Mathf.Sin(shootingAngle), -Mathf.Cos(shootingAngle));
            }
        }
        else
        {
            if (xAxis < 0 && transform.position.x > _horizontalMargin.x || xAxis > 0 && transform.position.x < _horizontalMargin.y)
                transform.Translate(xAxis*Time.deltaTime*speed, 0, 0);
        }

        if (_currentBall != null)
            _currentBall.transform.position = (Vector2)transform.position + shootingDirection;
    }
    
    private void LoadNewBall()
    {
        _currentBall = playerGameData.ballSetData.SpawnNewBall((Vector2)transform.position + shootingDirection, playerScore, disableCollision: true);
    }
    
    private void InitializePlayerData()
    {
        if (playerGameData == null)
        {
            Debug.LogError("The player doesn't have gameData");
        }
        _centeredPosition = transform.position;
        _horizontalMargin = new Vector2(_centeredPosition.x - boundariesFromCenter,
            _centeredPosition.x + boundariesFromCenter);
    }
}