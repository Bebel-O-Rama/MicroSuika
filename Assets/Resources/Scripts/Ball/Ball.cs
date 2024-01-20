using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Rigidbody2D rb2d;
    
    [FormerlySerializedAs("_tier")] public int tier;
    [FormerlySerializedAs("_scoreValue")] public int scoreValue;
    [FormerlySerializedAs("playerScore")] [FormerlySerializedAs("_playerScore")] public IntReference ballScoreRef;
    [FormerlySerializedAs("_ballSetData")] public BallSetData ballSetData;
    public GameModeData gameModeData;
    public Container container;
    
    public void SetBallData(BallSetData setData, int tierIndex, IntReference score = null, bool disableCollision = false)
    {
        ballSetData = setData;
        var ballData = ballSetData.GetBallData(tierIndex);
        if (ballData == null)
        {
            Debug.LogError("Trying to spawn a ball with a tier that doesn't exist");
            Destroy(gameObject);
        }

        spriteRenderer.sprite = ballSetData.ballSpriteData.GetBallSprite(tierIndex);
        transform.localScale = Vector3.one * ballData.scale;
        rb2d.mass = ballData.mass;

        tier = ballData.index;
        scoreValue = ballData.GetScoreValue();
        ballScoreRef = score;

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

    public void ClearBall()
    {
        ballScoreRef?.Variable.ApplyChange(scoreValue);
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
        {
            var newBall = Initializer.InstantiateBall(ballSetData, container, contactPosition);
            Initializer.SetBallParameters(newBall, tier + 1, ballScoreRef, ballSetData, container, false);
        }
    }
}