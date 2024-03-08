using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiSuika.GameLogic;
using MultiSuika.Utilities;
using UnityEngine;

namespace MultiSuika.Ball
{
    public class BallCollision : MonoBehaviour
    {
        // public int tier;
        // public BallInstance ballInstance;
        // public BallSetData ballSetData;
        // public Container container;
        // public IntReference ballScoreRef;
        // public BallTracker ballTracker;
        // public BallSpriteThemeData ballSpriteThemeData;
        //
        // public IGameModeManager gameModeManager;
        //
        //
        // public float impulseMultiplier;
        // public float impulseExpPower;
        // public float impulseRangeMultiplier;
        //
        // private bool _isBallCleared = false;
        //
        // public AK.Wwise.Event WwiseEventBallFuseT0;
        // public AK.Wwise.Event WwiseEventBallFuseT1;
        // public AK.Wwise.Event WwiseEventBallFuseT2;
        // public AK.Wwise.Event WwiseEventBallFuseT3;
        // public AK.Wwise.Event WwiseEventBallFuseT4;
        // public AK.Wwise.Event WwiseEventBallFuseT5;
        // public AK.Wwise.Event WwiseEventBallFuseT6;
        // public AK.Wwise.Event WwiseEventBallFuseT7;
        // public AK.Wwise.Event WwiseEventBallFuseT8;
        // public AK.Wwise.Event WwiseEventBallFuseT9;
        // public AK.Wwise.Event WwiseEventBallFuseT10;
        //
        //
        // private void OnCollisionEnter2D(Collision2D collision)
        // {
        //     if (!collision.transform.CompareTag("Ball")) 
        //         return;
        //     var otherBall = collision.gameObject.GetComponent<BallInstance>();
        //     if (otherBall.tier == tier && gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID() && !_isBallCleared && !otherBall.IsBallCleared())
        //     {
        //         FuseWithOtherBall(otherBall, collision.GetContact(0).point);
        //     }
        // }

        // public void SetCollisionEnter2DDelegate(System.Action func)
        // {
        //     onCollisionEnter2D += func;
        // }
        

        private Action<Collision2D> _onCollisionEnter2D;
        
        public void SubscribeCollisionEnter2D(Action<Collision2D> method, bool isSubscribing)
        {
            if (isSubscribing)
                _onCollisionEnter2D += method;
            else
                _onCollisionEnter2D -= method;
        }



        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.transform.CompareTag("Ball")) 
                return;
            _onCollisionEnter2D?.Invoke(collision);
            // var otherBall = collision.gameObject.GetComponent<BallInstance>();
            // if (otherBall.tier == tier && gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID() && !_isBallCleared && !otherBall.IsBallCleared())
            // {
            //     FuseWithOtherBall(otherBall, collision.GetContact(0).point);
            // }
        }

        //
        // private void FuseWithOtherBall(BallInstance other, Vector3 contactPosition)
        // {
        //     other.ClearBall();
        //     ballInstance.ClearBall();
        //     if (tier < ballSetData.GetMaxTier)
        //     {
        //         AddFusionImpulse(tier + 1, contactPosition);
        //         var newBall = Initializer.InstantiateBall(ballSetData, container,
        //             Initializer.WorldToLocalPosition(ContainerParent.transform, contactPosition));
        //         
        //         // TODO: Check if we can better fit that into the initialization encapsulation (we're setting in two different places)
        //         newBall.transform.SetLayerRecursively(gameObject.layer);
        //         
        //         Initializer.SetBallParameters(newBall, tier + 1, ballScoreRef, ballSetData, ballTracker, ballSpriteThemeData, container, gameModeManager);
        //         newBall.ballTracker.RegisterBall(newBall, container);
        //         CallFuseSFX(tier, newBall.gameObject);
        //     }
        //     gameModeManager.OnBallFusion(ballInstance);
        // }
        //
        // private void AddFusionImpulse(int newBallTier, Vector3 contactPosition)
        // {
        //     float impulseRadius = ballSetData.GetBallData(newBallTier).scale / 2f;
        //     var ballsInRange =
        //         from raycast in Physics2D.CircleCastAll(contactPosition, impulseRadius * container.ContainerParent.transform.localScale.x * impulseRangeMultiplier, Vector2.zero, Mathf.Infinity,
        //             LayerMask.GetMask("Ball"))
        //         select raycast.collider;
        //
        //
        //     foreach (var ball in ballsInRange)
        //     {
        //         Vector2 pushDirection = ball.transform.position - contactPosition;
        //         pushDirection.Normalize();
        //
        //         float pushIntensity = Mathf.Pow(Mathf.Abs(impulseRadius * impulseRangeMultiplier - Vector2.Distance(ball.ClosestPoint(contactPosition), contactPosition)) * impulseMultiplier, impulseExpPower);
        //
        //         ball.GetComponent<Rigidbody2D>().AddForce(pushIntensity * pushDirection, ForceMode2D.Impulse);
        //     }
        // }
        //
        // private void CallFuseSFX(int ballTier, GameObject newBallObj)
        // {
        //     switch (ballTier)
        //     {
        //         case 0:
        //             WwiseEventBallFuseT0.Post(newBallObj);
        //             break;
        //         case 1:
        //             WwiseEventBallFuseT1.Post(newBallObj);
        //             break;
        //         case 2:
        //             WwiseEventBallFuseT2.Post(newBallObj);
        //             break;
        //         case 3:
        //             WwiseEventBallFuseT3.Post(newBallObj);
        //             break;
        //         case 4:
        //             WwiseEventBallFuseT4.Post(newBallObj);
        //             break;
        //         case 5:
        //             WwiseEventBallFuseT5.Post(newBallObj);
        //             break;
        //         case 6:
        //             WwiseEventBallFuseT6.Post(newBallObj);
        //             break;
        //         case 7:
        //             WwiseEventBallFuseT7.Post(newBallObj);
        //             break;
        //         case 8:
        //             WwiseEventBallFuseT8.Post(newBallObj);
        //             break;
        //         case 9:
        //             WwiseEventBallFuseT9.Post(newBallObj);
        //             break;
        //         case 10:
        //             WwiseEventBallFuseT10.Post(newBallObj);
        //             break;
        //     }
        // }
        
    }
}
