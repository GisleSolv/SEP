using System;
using System.Collections;
using UnityEngine;
using PerfectlyParanormal.Bezier;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance { get { return m_instance; } }

    public static CameraSam Cam;

    public int CurrentPlaythrough = 0;

    public BezierPathPlayer CameraPath;

    public Transform PlayerStart;
    public Player Player;

    private PlaythroughSpecificObject[] m_pObjects;

    public delegate void OnLevelResetEvent();

    public static event OnLevelResetEvent OnLevelReset, OnLevelStart;

    public string NextLevel;
    public int LevelsToWin = 0;

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;
        else Destroy(gameObject);

        AudioManager.LoadFabric();
    }

    void Start()
    {
        m_pObjects = FindObjectsOfType<PlaythroughSpecificObject>();

        StartCoroutine(DelayedRestart(0, 0.5f));


    }

    public void SetPlaythrough(int p)
    {
        for (int i = 0; i < m_pObjects.Length; i++)
        {
            m_pObjects[i].SetPlaythrough(p);
        }

    }

    public void OnPlayerDied()
    {
        if (CameraPath)
            CameraPath.Stop();
        StartCoroutine(DelayedRestart(-1, 1));
    }

    IEnumerator DelayedRestart(int n, float fadeD)
    {
        
        Player.SetDead(true);
        Player.Body.simulated = false;
        yield return new WaitForSeconds(fadeD);
        Cam.FadeOut(0.5f);

        yield return new WaitForSeconds(0.5f);

        if (CameraPath)
            CameraPath.MoveToPosition(0);
        Player.PreReset(PlayerStart.position);

        if (OnLevelReset != null)
            OnLevelReset();

        Cam.FadeIn(0.5f);
        yield return new WaitForSeconds(1f);
        
        CurrentPlaythrough +=n;
        if (CurrentPlaythrough < 0) CurrentPlaythrough = 0;
       
        SetPlaythrough(CurrentPlaythrough);


        

        yield return new WaitForSeconds(0.5f);

        if (OnLevelStart != null)
            OnLevelStart();
        ResetLevel(CurrentPlaythrough + n);
       
    }

    void ResetLevel(int c)
    {

        Player.Reset();
        if (CameraPath)
            CameraPath.PlayPath(0);
        
    }

    public void OnLevelComplete()
    {
        if(CurrentPlaythrough>=LevelsToWin)
        {
            OnLevelReset = null;
            OnLevelStart = null;
            Cam = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene(NextLevel);
        }
        else
            StartCoroutine(DelayedRestart(1, 0.5f));
    }


}

