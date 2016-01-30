using System;
using System.Collections.Generic;
using UnityEngine;
using PerfectlyParanormal.FSM;


public class Jellyfish : PerfectlyParanormal.FSM.StateMachineBehaviour, IEnemy
{

    private Animator m_anim;
    private float m_initScale;


    void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_initScale = transform.localScale.x;
    }


    public void Kill()
    {
        m_anim.Play("jelly_attack");
    }

    public void OnApplyDamage()
    {
        
    }

    public void OnPlayerHit(Player p, Collision2D c)
    {
        Vector3 s = transform.localScale;
        if (p.transform.position.x < transform.position.x)
            s.x = -m_initScale;
        else s.x = m_initScale;
        transform.localScale = s;
    }
}

