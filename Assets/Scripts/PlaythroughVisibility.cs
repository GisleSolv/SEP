using System;
using System.Collections;
using UnityEngine;

public class PlaythroughVisibility : PlaythroughSpecificObject
{
    
    private SpriteRenderer m_sprite;

    void Awake()
    {
        m_sprite = GetComponent<SpriteRenderer>();
    }

    public override void SetPlaythrough(int p)
    {
        if(p>=TriggerOnPlayNum && m_currentPlayIndex < p)
        {
            m_sprite.enabled = false;
        }
        else if(p<TriggerOnPlayNum && m_currentPlayIndex>=p)
        {
            m_sprite.enabled = true;
        }

        base.SetPlaythrough(p);
    }

}

