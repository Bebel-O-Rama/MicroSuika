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
    private float scale;
    private float mass;
    private Sprite sprite;
    private int tierIndex;

    public void SetBallData(BallData ballData, int index, Vector3 position)
    {
        scale = ballData.scale;
        mass = ballData.mass;
        sprite = ballData.sprite;
        tierIndex = index;

        spriteRenderer.sprite = sprite;
        transform.localScale = Vector3.one*scale;
        rb2d.mass = mass;
        transform.position = position;
    }

    public int GetBallTier()
    {
        return tierIndex;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Ball")) return;
        var otherBall = collision.gameObject.GetComponent<Ball>();
        if (otherBall.GetBallTier() == tierIndex)
        {
            FuseWithOtherBall(otherBall, collision.transform.position);  
        }
    }

    private void FuseWithOtherBall(Ball other, Vector3 contactPosition)
    {
        Debug.Log("FU-SIOOON");
    }
}
