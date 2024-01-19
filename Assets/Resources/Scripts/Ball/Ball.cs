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
    private int _tier;
    private int _scoreValue;
    private IntReference _playerScore;
    private BallSetData _ballSetData;

    public void SetBallData(BallSetData setData, int tierIndex, IntReference score = null, bool disableCollision = false)
    {
        _ballSetData = setData;
        var ballData = _ballSetData.GetBallData(tierIndex);
        if (ballData == null)
        {
            Debug.LogError("Trying to spawn a ball with a tier that doesn't exist");
            Destroy(gameObject);
        }

        spriteRenderer.sprite = _ballSetData.ballSpriteData.GetBallSprite(tierIndex);
        transform.localScale = Vector3.one * ballData.scale;
        rb2d.mass = ballData.mass;

        _tier = ballData.index;
        _scoreValue = ballData.GetScoreValue();
        _playerScore = score;

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

    public int GetBallTier() => _tier;

    public void ClearBall()
    {
        _playerScore?.Variable.ApplyChange(_scoreValue);
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
        if (otherBall.GetBallTier() == _tier && gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID())
        {
            FuseWithOtherBall(otherBall, collision.GetContact(0).point);
        }
    }

    private void FuseWithOtherBall(Ball other, Vector3 contactPosition)
    {
        other.ClearBall();
        ClearBall();
        if (_tier < _ballSetData.GetMaxTier)
            _ballSetData.SpawnNewBall(contactPosition, _tier + 1, _playerScore);
    }
}