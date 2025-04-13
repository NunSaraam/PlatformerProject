using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapDisabler : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TilemapDisabler>().enabled = false;
    }
}
