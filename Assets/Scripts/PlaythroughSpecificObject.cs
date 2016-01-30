using System;
using System.Collections;
using UnityEngine;

public class PlaythroughSpecificObject : MonoBehaviour
{
    public int TriggerOnPlayNum = 0;
    protected int m_currentPlayIndex = 0;

    public virtual void SetPlaythrough(int p)
    {
        m_currentPlayIndex = p;
    }

}

