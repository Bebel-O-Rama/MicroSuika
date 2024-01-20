using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Ball/Ball Set Data")]
public class BallSetData : ScriptableObject
{
    public string ballSetName;
    [Header("The smaller the index position of the ball, the smaller its radius should be")]
    [Header("ALSO, you can't have null here, you need to drag and drop the BallData SO on the list")]
    public List<BallData> ballSetData;

    public BallSpriteThemeData ballSpriteData;
    
    public GameObject ballPrefab;
    
    private float totalWeight;
        
    public BallData GetBallData(int index) => ballSetData.Count > index ? ballSetData[index] : null;
    public int GetMaxTier => ballSetData.Count - 1;

    public Ball SpawnNewBall(Vector3 position, IntReference score, int tierIndex, bool disableCollision = false)
    {
        return SpawnBall(position, tierIndex, score, disableCollision);
    }
    
    public Ball SpawnNewBall(Vector3 position, IntReference score, bool useWeightedRandom = true, bool disableCollision = false)
    {
        return SpawnBall(position, GetRandomBallTier(useWeightedRandom), score, disableCollision);
    }
    
    public int GetRandomBallTier(bool usingWeight = true)
    {
        if (!usingWeight)
            return Random.Range(0, ballSetData.Count - 1);
        
        float randValue = Random.Range(0f, 1f);
        foreach (var ballData in ballSetData)
        {
            randValue -= ballData.spawnChance / totalWeight;
            if (randValue <= 0)
                return ballData.index;
        }
        return 0;
    }

    private Ball SpawnBall(Vector2 position, int tierIndex, IntReference score, bool disableCollision)
    {
        GameObject spawnedBall = Instantiate(ballPrefab, position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))) as GameObject;
        var newBall = spawnedBall.GetComponent<Ball>();
        newBall.SetBallData(this, tierIndex, score, disableCollision);
        return newBall;
    }
    
    private void TestingAndCleaningSet()
    {
        // It's not flawless, but at least it takes care of null elements and duplicates. The OnValidate should already take care of the order of the BallData
        var tempSet = new List<BallData>();
        foreach (var ballData in ballSetData)
        {
            if (ballData != null && !tempSet.Contains(ballData))
                tempSet.Add(ballData);
        }
        // if (tempSet != ballSetData)
        //     Debug.LogWarning("Modifications have been made to the BallSetData " + ballSetName);
        ballSetData = tempSet;
    }

    private void SetWeight() => totalWeight = ballSetData.Sum(b => b.spawnChance);

    private void OnValidate()
    {
        ballSetData.RemoveAll(item => item == null);
        ballSetData = ballSetData.OrderBy(ball => ball.index).ToList();
    }

    private void OnEnable()
    {
        TestingAndCleaningSet();
        SetWeight();
        if (ballPrefab == null)
            Debug.LogError("The Ball Prefab in the BallSetData is null!");
    }
}