using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Rigidbody2D rb2d;
    private int tier;
    private int scoreValue;
    private IntReference playerScore;
    private BallSetData ballSetData;

    public void SetBallData(BallSetData setData, int tierIndex, IntReference score = null, bool disableCollision = false)
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
        {
            rb2d.simulated = false;
            return;
        }

        ApplyRotationForce();
    }

    public void EnableCollision()
    {
        rb2d.simulated = true;
        ApplyRotationForce();
    }

    public int GetBallTier() => tier;
    public float GetBallRadius() => transform.localScale.x / 2f;
    public Vector2 GetBallPosition() => transform.position;

    public void ClearBall()
    {
        playerScore?.Variable.ApplyChange(scoreValue);
        rb2d.simulated = false;
        Destroy(gameObject);
    }

    private void ApplyRotationForce()
    {
        var zRotationValue = Random.Range(0.1f, 0.2f) * (Random.Range(0, 2) * 2 - 1);
        rb2d.AddTorque(zRotationValue, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Ball")) return;
        var otherBall = collision.gameObject.GetComponent<Ball>();
        if (otherBall.GetBallTier() == tier && gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID())
        {
            FuseWithOtherBall(otherBall, collision.GetContact(0).point);
        }
    }

    private void FuseWithOtherBall(Ball other, Vector3 contactPosition)
    {
        other.ClearBall();
        ClearBall();
        if (tier < ballSetData.GetMaxTier)
            ballSetData.SpawnNewBall(contactPosition, tier + 1, playerScore);
    }
}