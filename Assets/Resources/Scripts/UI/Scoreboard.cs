using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] public TMP_Text tmp;
    [SerializeField] public IntReference playerScore;

    private void Update()
    {
        tmp.text = string.Format($"Score : {playerScore.Value}");
    }
}
