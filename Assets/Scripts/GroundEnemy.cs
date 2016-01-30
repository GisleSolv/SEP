using System;
using System.Collections;
using UnityEngine;
using PerfectlyParanormal.FSM;


public class GroundEnemy : PerfectlyParanormal.FSM.StateMachineBehaviour, IEnemy
{
    public enum GroundEnemyStates
    {
        Left,
        Right,
        Attack,
        Dead,
        None
    }

    public GroundEnemyStates InitState;

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

        InitializeStates<GroundEnemyStates>(typeof(EnemyStateMapping));

        m_initPos = transform.position;
        m_initScale = transform.localScale.x;
        //ChangeState(InitState);
        
        GameManager.OnLevelReset += OnReset;
        GameManager.OnLevelStart += OnStart;
    }

    void OnReset()
    {
        SetState(GroundEnemyStates.None);
        transform.position = m_initPos;
        m_collider.enabled = true;
        Vector3 s = transform.localScale;
        if (InitState == GroundEnemyStates.Left)
            s.x = -m_initScale;
        else s.x = m_initScale;
        transform.localScale = s;
        m_anim.Play("cat_walk");
        
    }

    void OnStart()
    {
        ChangeState(InitState);
    }

    void MoveDelta(float d)
    {
        Vector3 pos = transform.position;
        pos.x += d*Time.deltaTime;
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
            ChangeState(GroundEnemyStates.Right);
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
            ChangeState(GroundEnemyStates.Left);
    }

    IEnumerator Attack_Enter(Enum prevState, Hashtable data)
    {
        Vector3 s = transform.localScale;
        if (transform.position.x < m_player.transform.position.x)
            s.x = m_initScale;
        else s.x = -m_initScale;
        transform.localScale = s;

        m_anim.Play("cat_attack");
        m_player.Kill("carl_dieCat", new Vector3());

        yield return null;
        
    }

    void Attack_LateEnter(Enum ps)
    {
        ChangeState(ps);
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

        for (int i=0; i<c.contacts.Length; i++)
        {
            if(c.contacts[i].normal.y>=0.7f)
            {
                m_collider.enabled = false;
                m_anim.Play("cat_die");
                Vector2 v = p.Body.velocity;
                v.y = 25;
                p.Body.velocity = v;
                ChangeState(GroundEnemyStates.Dead);
            }
            else ChangeState(GroundEnemyStates.Attack);
        }


        
    }

    public void OnApplyDamage()
    {
        
    }
}

