using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Ball : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Rigidbody2D rb2d;
    private int tier;
    private int scoreValue;
    private IntReference playerScore;
    private BallSetData ballSetData;

    public void SetBallData(BallSetData setData, int tierIndex, IntReference score)
    {
        ballSetData = setData;
        var ballData = ballSetData.GetBallData(tierIndex);
        if (ballData == null)
        {
            Debug.LogError("Trying to spawn a ball with a tier that doesn't exist");
            Destroy(gameObject);
        }
        spriteRenderer.sprite = ballData.sprite;
        transform.localScale = Vector3.one * ballData.scale;
        rb2d.mass = ballData.mass;
        
        tier = ballData.index;
        scoreValue = ballData.GetScoreValue();
        playerScore = score;
    }

    public int GetBallTier() => tier;

    public void ClearBall()
    {
        playerScore?.Variable.ApplyChange(scoreValue);
        Destroy(gameObject);
    }

private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Ball")) return;
        var otherBall = collision.gameObject.GetComponent<Ball>();
        if (otherBall.GetBallTier() == tier)
        {
            FuseWithOtherBall(otherBall, collision.transform.position);
        }
    }

    private void FuseWithOtherBall(Ball other, Vector3 contactPosition)
    {
        other.ClearBall();
        
    }
}