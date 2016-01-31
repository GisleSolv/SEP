using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Intro : MonoBehaviour
{

    public Image[] Slides;
    public int CurImg = 0;
    public string NextLevel;

    void Start()
    {
        SetImg();

        AudioManager.LoadFabric();
    }

    void Update()
    {
        if(Input.GetButtonDown("Horizontal"))
        {
            if(Input.GetAxis("Horizontal")>0)
            {
                CurImg++;
                if (CurImg >= Slides.Length)
                {
                    AudioManager.PlaySound("Narrator");
                    UnityEngine.SceneManagement.SceneManager.LoadScene(NextLevel);
                }
                else SetImg();
            }
            else
            {
                CurImg--; SetImg();
            }
        }
    }

    void SetImg()
    {
        for (int i = 0; i < Slides.Length; i++)
        {
            if (i == CurImg)
                Slides[i].enabled = true;
            else Slides[i].enabled = false;
        }
    }
}

