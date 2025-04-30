using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTest : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("HighScore", 12345);
        PlayerPrefs.Save();
        int best = PlayerPrefs.GetInt("HighScore");
        Debug.Log($"{best}");
    }

    void Update()
    {
        
    }
}
