using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBallSpawner : MonoBehaviour
{
    [SerializeField] public BallSetData ballSetData;

    [Tooltip("It's not ideal, but it simplifies my job a bit so we need to pass an IntReference here now")]
    public IntReference debugScore;

    private Object ballObj;

    // Start is called before the first frame update
    void Start()
    {
        if (ballSetData == null)
            Debug.LogError("The ball data set in the DebugBallSpawner is null");
        ballObj = Resources.Load("PF_Ball");
        if (ballObj == null)
            Debug.LogError("The DebugBallSpawner can't load the ball Prefab (PF_Ball)");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 2f;

            Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos);
            GameObject spawnedBall = Instantiate(ballObj, objectPos, Quaternion.identity) as GameObject;
            Ball newBall = spawnedBall.GetComponent<Ball>();

            if (Input.GetKey(KeyCode.Alpha1))
                newBall.SetBallData(ballSetData, 0, debugScore);
            else if (Input.GetKey(KeyCode.Alpha2))
                newBall.SetBallData(ballSetData, 1, debugScore);
            else if (Input.GetKey(KeyCode.Alpha3))
                newBall.SetBallData(ballSetData, 2, debugScore);
            else if (Input.GetKey(KeyCode.Alpha4))
                newBall.SetBallData(ballSetData, 3, debugScore);
            else if (Input.GetKey(KeyCode.Alpha5))
                newBall.SetBallData(ballSetData, 4, debugScore);
            else
            {
                Destroy(spawnedBall);
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 2f;

            Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos);
            GameObject spawnedBall = Instantiate(ballObj, objectPos, Quaternion.identity) as GameObject;
            Ball newBall = spawnedBall.GetComponent<Ball>();

            newBall.SetBallData(ballSetData, ballSetData.GetRandomBallTier(), debugScore);
        }
    }
}