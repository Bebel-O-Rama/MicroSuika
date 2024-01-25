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

    public int tier;
    public int scoreValue;
    public IntReference ballScoreRef;
    public BallSetData ballSetData;
    public BallSpriteThemeData ballSpriteThemeData;
    public Container container;

    public void EnableCollision()
    {
        rb2d.simulated = true;
        ApplyRotationForce();
    }

    private int GetBallTier() => tier;

    private void ClearBall()
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
            var newBall = Initializer.InstantiateBall(ballSetData, container,
                Initializer.WorldToLocalPosition(container.containerParent.transform, contactPosition));
            Initializer.SetBallParameters(newBall, tier + 1, ballScoreRef, ballSetData, ballSpriteThemeData, container, false);
        }
    }
}