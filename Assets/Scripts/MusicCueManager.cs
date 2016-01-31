using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fabric;

/// <summary>
/// Singleton for controlling music audio in the game. This script is attached to 'Resources/Prefabs/Audio.prefab'
/// </summary>
public class MusicCueManager : MonoBehaviour
{

    private static MusicCueManager m_instance;

    public static MusicCueManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<MusicCueManager>();
                
            }

            return m_instance;
        }
    }

    /// <summary>
    /// List of music event names from Fabric 
    /// </summary>
    public List<string> MusicCues;

    private int m_currentCue=-1;

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;

        else if(m_instance!=this) Destroy(this.gameObject);
    }


    /// <summary>
    /// Sets which music track should play
    /// </summary>
    /// <param name="index">Index in MusicCues list, '-1' means stop</param>
    /// <param name="startTime">Start time in seconds</param>
    public void SetMusicCue(int index, float startTime=0f)
    {
        if(m_currentCue==index) return;

        if (m_currentCue > -1)
            Fabric.EventManager.Instance.PostEvent(MusicCues[m_currentCue], EventAction.StopSound);

        if (index > -1)
        {
            Fabric.EventManager.Instance.PostEvent(MusicCues[index], EventAction.PlaySound);
            Fabric.EventManager.Instance.PostEvent(MusicCues[index], EventAction.SetTime, startTime);
            
        }
        if (index < 0)
            StopMusicCue(m_currentCue);

        m_currentCue = index;
    }

    /// <summary>
    /// Stops music track
    /// </summary>
    /// <param name="index">Index in AmbientCues list</param>
    public void StopMusicCue(int index, bool ignoreFade=false)
    {
        if (m_currentCue == index && m_currentCue>-1)
        {
            Fabric.EventManager.Instance.PostEvent(MusicCues[m_currentCue], EventAction.StopSound, ignoreFade);
            m_currentCue = -1;
        }
    }

    /// <summary>
    /// Stops whichever track is playing
    /// </summary>
    public void StopCurrentCue()
    {
        if(m_currentCue<0) return;

        Fabric.EventManager.Instance.PostEvent(MusicCues[m_currentCue], EventAction.StopSound, true);

        m_currentCue=-1;
    }

    /// <summary>
    /// Sets playback time of music track
    /// </summary>
    /// <param name="index">Index in AmbientCues list</param>
    /// <param name="time">Time in seconds</param>
    public void SetCueTime(int index, float time)
    {
        if (m_currentCue != index || index==-1) return;

        Fabric.EventManager.Instance.PostEvent(MusicCues[m_currentCue], EventAction.SetTime, time);
    }

}
