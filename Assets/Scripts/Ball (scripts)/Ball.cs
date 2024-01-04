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
    private bool hasBeenCleared = false;

    public void SetBallData(BallSetData setData, int tierIndex, IntReference score, bool disableCollision = false)
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

        if (disableCollision)
            rb2d.simulated = false;
    }

    public void EnableCollision() => rb2d.simulated = true;

    public int GetBallTier() => tier;

    public void ClearBall()
    {
        playerScore?.Variable.ApplyChange(scoreValue);
        hasBeenCleared = true;
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Ball") || hasBeenCleared) return;
        var otherBall = collision.gameObject.GetComponent<Ball>();
        if (otherBall.GetBallTier() == tier)
        {
            FuseWithOtherBall(otherBall, collision.GetContact(0).point);
        }
    }

    private void FuseWithOtherBall(Ball other, Vector3 contactPosition)
    {
        other.ClearBall();
        ballSetData.SpawnNewBall(contactPosition, tier + 1, playerScore);
        ClearBall();
    }
}