using System;
using System.Collections.Generic;
using UnityEngine;


public class MusicCue : MonoBehaviour
{
    public int Cue = 0;
    public bool PlayOnStart = false;
    bool m_played;

    void Awake()
    {
        if(!PlayOnStart)
        {
            GameManager.OnLevelStart += OnStart;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (PlayOnStart && !m_played)
        {
            if (AudioManager.FabricLoaded)
            {
                MusicCueManager.Instance.SetMusicCue(Cue);
                m_played = true;
            }
        }
    }

    void OnStart()
    {
        MusicCueManager.Instance.StopCurrentCue();
        MusicCueManager.Instance.SetMusicCue(Cue);
        GameManager.OnLevelStart -= OnStart;
    }

}

