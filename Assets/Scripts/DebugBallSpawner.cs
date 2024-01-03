using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBallSpawner : MonoBehaviour
{
    [SerializeField] public GameData gameData;

    private Object ballObj;

    // Start is called before the first frame update
    void Start()
    {
        if (gameData == null)
            Debug.LogError("The gameData in the DebugBallSpawner is null");
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
                newBall.SetBallData(gameData.GetBallData(0), 0, objectPos);
            else if (Input.GetKey(KeyCode.Alpha2))
                newBall.SetBallData(gameData.GetBallData(1), 1, objectPos);
            else if (Input.GetKey(KeyCode.Alpha3))
                newBall.SetBallData(gameData.GetBallData(2), 2, objectPos);
            else if (Input.GetKey(KeyCode.Alpha4))
                newBall.SetBallData(gameData.GetBallData(3), 3, objectPos);
            else if (Input.GetKey(KeyCode.Alpha5))
                newBall.SetBallData(gameData.GetBallData(4), 4, objectPos);
            else
            {
                Destroy(spawnedBall);
            }
            
        }
    }
}
