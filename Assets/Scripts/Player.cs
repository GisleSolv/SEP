using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D m_body;
    public Rigidbody2D Body { get { return m_body; } }
    private Animator m_anim;

    public float Accelration;
    public float MaxVelocity;
    public float JumpVelocity;
    public float GroundFalloff;
    public float AirFalloff;

    public Vector2 ColBoxUL, ColBoxLR;
    public LayerMask GroundLayer;
    private bool m_grounded;

    float m_initScale;

    private bool m_dead = false;

    void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_anim = GetComponentInChildren<Animator>();
        m_initScale = m_anim.transform.localScale.x;
    }

    public void PreReset(Vector3 p)
    {
        transform.position = p;
        m_anim.SetBool("Grounded", false);
        m_anim.SetBool("Moving", false);
        m_anim.SetFloat("ySpeed", 0);
        m_anim.SetBool("Crouch", false);
        m_anim.Play("carl_idle");
    }

    public void Reset()
    {
        m_dead = false;
        

        foreach (Collider2D c in GetComponents<Collider2D>())
            c.enabled = true;

        m_body.velocity = new Vector2(0, 0);
        m_body.simulated = true;
        m_body.gravityScale = 5;
    }

    void FixedUpdate()
    {
        if (m_dead) return;

        Vector2 pos = transform.position;
        m_grounded = Physics2D.OverlapArea(pos + ColBoxUL, pos + ColBoxLR, GroundLayer) != null;


        Vector2 vel = m_body.velocity;

        float moveX = 0;
        if(Input.GetButton("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                moveX = -Accelration;
            }
            else moveX = Accelration;
        }
        else
        {
            if (m_grounded) vel.x *= GroundFalloff;
            else vel.x *= AirFalloff;
        }
        

        vel.x += moveX;

        if (vel.x > MaxVelocity) vel.x = MaxVelocity;
        else if (vel.x < -MaxVelocity) vel.x = -MaxVelocity;

        if (moveX != 0) {
            Vector3 scale = m_anim.transform.localScale;
            if (vel.x < 0)
                scale.x = -m_initScale;
            else scale.x = m_initScale;

            m_anim.transform.localScale = scale;
        }

        m_body.velocity = vel;

        m_anim.SetBool("Grounded", m_grounded);
        m_anim.SetBool("Moving", moveX != 0);
        m_anim.SetFloat("ySpeed", m_body.velocity.y);
        m_anim.SetBool("Crouch", Input.GetAxis("Vertical") < 0);

    }

    void Update()
    {
        if (m_dead) return;

        if (Input.GetButtonDown("Jump") && m_grounded)
        {
            Vector2 vel = m_body.velocity;
            vel.y = JumpVelocity;
            m_body.velocity = vel;
            m_anim.SetTrigger("Jump");
        }
    }


    void OnCollisionEnter2D(Collision2D c)
    {
        if(c.collider.tag=="Enemy")
        {
            IEnemy e = c.collider.GetComponent<IEnemy>();
            if (e != null)
                e.OnPlayerHit(this, c);
                
        }
    }

    public void SetDead(bool d)
    {
        m_dead = d;
        m_body.gravityScale = 0;
    }

    public void Kill(string anim, Vector3 pos)
    {
        m_dead = true;
        m_anim.Play(anim);
        m_body.velocity = new Vector2(0, 0);

        foreach (Collider2D c in GetComponents<Collider2D>())
            c.enabled = false;

        m_body.simulated = false;
        GameManager.Instance.OnPlayerDied();
    }

    void OnDrawGizmos()
    {
        Vector3 p = transform.position;

        Gizmos.DrawLine(new Vector3(p.x + ColBoxUL.x, p.y + ColBoxUL.y, p.z), new Vector3(p.x + ColBoxLR.x, p.y + ColBoxUL.y, p.z));
        Gizmos.DrawLine(new Vector3(p.x + ColBoxLR.x, p.y + ColBoxUL.y, p.z), new Vector3(p.x + ColBoxLR.x, p.y + ColBoxLR.y, p.z));
        Gizmos.DrawLine(new Vector3(p.x + ColBoxLR.x, p.y + ColBoxLR.y, p.z), new Vector3(p.x + ColBoxUL.x, p.y + ColBoxLR.y, p.z));
        Gizmos.DrawLine(new Vector3(p.x + ColBoxUL.x, p.y + ColBoxLR.y, p.z), new Vector3(p.x + ColBoxUL.x, p.y + ColBoxUL.y, p.z));
    }

}

