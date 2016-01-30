using System;
using System.Collections.Generic;
using UnityEngine;


public class WinZone : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.tag=="Player")
        {
            GameManager.Instance.OnLevelComplete();
        }
    }

}

