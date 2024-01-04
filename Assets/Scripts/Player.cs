using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Specific Parameters")]
    [Min(0f)] [SerializeField] public float speed;
    [Min(0f)] [SerializeField] public float reloadCooldown;
    [Min(0f)] [SerializeField] public float yBallDelta;
    [Min(0f)] [SerializeField] public float boundariesFromCenter;
    
    [Header("General Game Parameters")]
    [SerializeField] public GameData gameData;
    [SerializeField] public IntReference playerScore;
    
    private Vector3 centeredPosition;
    private Vector2 horizontalMargin;
    private Queue<Ball> nextBalls;
    private Ball currentBall;
    private Vector3 ballDelta;

    private void Start()
    {
        InitializePlayerData();
        LoadNewBall();
    }

    private void Update()
    {
        UpdatePlayerPosition(Input.GetAxis("Horizontal"));
        if (Input.GetKeyDown(KeyCode.Space) && currentBall != null)
            DropBall();
    }

    private void DropBall()
    {
        currentBall.EnableCollision();
        currentBall = null;
        Invoke("LoadNewBall", reloadCooldown);
    }

    private void LoadNewBall()
    {
        currentBall = gameData.ballSetData.SpawnNewBall(transform.position + ballDelta, playerScore, disableCollision: true);
    }
    
    private void InitializePlayerData()
    {
        if (gameData == null)
        {
            Debug.LogError("The player doesn't have gameData");
        }
        ballDelta = new Vector3(0f, -yBallDelta, 0f);
        centeredPosition = transform.position;
        horizontalMargin = new Vector2(centeredPosition.x - boundariesFromCenter,
            centeredPosition.x + boundariesFromCenter);
    }

    private void UpdatePlayerPosition(float xAxis)
    {
        if (xAxis == 0) return;
        if (xAxis < 0 && transform.position.x > horizontalMargin.x || xAxis > 0 && transform.position.x < horizontalMargin.y)
            transform.Translate(xAxis*Time.deltaTime*speed, 0, 0);
        if (currentBall != null)
            currentBall.transform.position = transform.position + ballDelta;

    }
}
