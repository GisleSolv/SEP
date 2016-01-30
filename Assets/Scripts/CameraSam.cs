using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Main camera script, registeres with GameManager when level starts. 
/// Has references to eye-effect script and follow script
/// Has fading functionality
/// This script may change
/// </summary>
public class CameraSam : MonoBehaviour {




    private Texture2D FadeTexture;
    public float m_blackScreenAlpha { get; private set; }
    public bool FadeInAtStart = true;
    public float StartFadeAlpha=1;
    public Color StartFadeColor = Color.black;
    private Action m_fadeCallback;

    LTDescr m_ease;

    private Camera m_camera;
    public Camera Camera { get { return m_camera; } }

    void Awake()
    {

        m_camera = GetComponent<Camera>();
        GameManager.Cam = this;
    }

	// Use this for initialization
    protected void Start()
    {
        FadeTexture = new Texture2D(1, 1);
        FadeTexture.SetPixel(0, 0, StartFadeColor);
        FadeTexture.Apply();
        
        m_blackScreenAlpha = 1;


        
        
        if (FadeInAtStart) FadeIn(2f);
        else SetFadeAlpha(StartFadeAlpha);


       


    }

    public void SetFadeColor(Color c)
    {
        FadeTexture.SetPixel(0, 0, c);
        FadeTexture.Apply();
    }

    

    void OnGUI()
    {

        if (m_blackScreenAlpha > 0f)
        {
            Color c = new Color(1f, 1f, 1f, m_blackScreenAlpha);
            GUI.color = c;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture);

        }
    }

   

    public void FadeIn(float dur, LeanTweenType trans = LeanTweenType.easeInCubic, Action callback = null)
    {
        if (m_ease != null && LeanTween.isTweening(m_ease.uniqueId))
            LeanTween.cancel(m_ease.uniqueId);

        m_ease = LeanTween.value(gameObject, OnFade, m_blackScreenAlpha, 0, dur).setEase(trans);
        m_ease.onComplete = callback;

    }

    void OnFade(float v)
    {
        m_blackScreenAlpha = v;
    }

    public void FadeOut(float dur, LeanTweenType trans = LeanTweenType.easeOutCubic, Action callback = null)
    {
        if (m_ease != null && LeanTween.isTweening(m_ease.uniqueId))
            LeanTween.cancel(m_ease.uniqueId);

        m_ease = LeanTween.value(gameObject, OnFade, m_blackScreenAlpha, 1, dur).setEase(trans);
        m_ease.onComplete = callback;
    }

    public void SetFadeAlpha(float v)
    {
        m_blackScreenAlpha = v;
    }

    

}
