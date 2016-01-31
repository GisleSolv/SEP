using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy : PerfectlyParanormal.FSM.StateMachineBehaviour, IEnemy
{
    public enum AirEnemyStates
    {
        Left,
        Right,
        None
    }

    public AirEnemyStates InitState;

    public float LeftBorder, RightBorder;
    private Vector3 m_initPos;

    public float Speed = 1;
    private float m_initScale;

    private Player m_player;
    private Animator m_anim;
    private Collider2D m_collider;

    void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();

        InitializeStates<AirEnemyStates>(typeof(EnemyStateMapping));

        m_initPos = transform.position;
        m_initScale = -transform.localScale.x;
        //ChangeState(InitState);

        GameManager.OnLevelReset += OnReset;
        GameManager.OnLevelStart += OnStart;
    }

    void OnReset()
    {
        SetState(AirEnemyStates.None);
        transform.position = m_initPos;
        m_collider.enabled = true;
        Vector3 s = transform.localScale;
        if (InitState == AirEnemyStates.Left)
            s.x = -m_initScale;
        else s.x = m_initScale;
        transform.localScale = s;

    }

    void OnStart()
    {
        ChangeState(InitState);
    }

    void MoveDelta(float d)
    {
        Vector3 pos = transform.position;
        pos.x += d * Time.deltaTime;
        transform.position = pos;
    }

    void Left_LateEnter(Enum ps)
    {
        Vector3 s = transform.localScale;
        s.x = -m_initScale;
        transform.localScale = s;
    }

    void Left_Update()
    {
        MoveDelta(-Speed);
        if (transform.position.x < m_initPos.x + LeftBorder)
            ChangeState(AirEnemyStates.Right);
    }

    void Right_LateEnter(Enum ps)
    {
        Vector3 s = transform.localScale;
        s.x = m_initScale;
        transform.localScale = s;
    }

    void Right_Update()
    {
        MoveDelta(Speed);
        if (transform.position.x > m_initPos.x + RightBorder)
            ChangeState(AirEnemyStates.Left);
    }



    void OnDrawGizmos()
    {
        Vector3 l = transform.position;
        l.x += LeftBorder;
        Vector3 r = transform.position;
        r.x += RightBorder;
        Gizmos.DrawLine(l, r);
    }

    public void OnPlayerHit(Player p, Collision2D c)
    {
        m_player = p;


        if(!m_player.Crouching || !m_player.Grounded)
        {
            m_player.Kill("carl_dieCat", new Vector3());
        }





    }

    public void OnApplyDamage()
    {

    }

}

