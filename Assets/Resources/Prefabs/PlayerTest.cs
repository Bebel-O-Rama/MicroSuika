using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [SerializeField] public float speed;
    
    public void MoveH(float x)
    {
        transform.Translate(x * Time.deltaTime * speed, 0, 0);
        
    }
    
    public void MoveV(int y)
    {
        transform.Translate(0, y * Time.deltaTime * speed * 10, 0);
        
    }
}
