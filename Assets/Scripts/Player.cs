using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Specific Parameters")]
    [Min(0f)] [SerializeField] public float speed;
    [Min(0f)] [SerializeField] public float boundariesFromCenter;
    [Header("General Game Parameters")]
    [SerializeField] public GameData ballSet;

    private Vector3 centeredPosition;
    private Vector2 horizontalMargin;
    private Queue<Ball> nextBalls;
    private Ball currentBall;

    private GameObject ballPrefab;

    private void Start()
    {
        InitializePlayerData();
    }

    private void Update()
    {
        UpdatePlayerPosition(Input.GetAxis("Horizontal"));
    }

    private void InitializePlayerData()
    {
        ballPrefab = Resources.Load("PF_Ball") as GameObject;
        if (ballSet == null)
        {
            Debug.LogError("The player doesn't have gameData");
        }

        centeredPosition = transform.position;
        horizontalMargin = new Vector2(centeredPosition.x - boundariesFromCenter,
            centeredPosition.x + boundariesFromCenter);
    }

    private void UpdatePlayerPosition(float xAxis)
    {
        if (xAxis == 0) return;
        if (xAxis < 0 && transform.position.x > horizontalMargin.x || xAxis > 0 && transform.position.x < horizontalMargin.y)
            transform.Translate(xAxis*Time.deltaTime*speed, 0, 0);
    }
}
