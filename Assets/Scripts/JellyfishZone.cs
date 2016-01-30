using System;
using System.Collections;
using UnityEngine;


public class JellyfishZone : MonoBehaviour
{

    public Jellyfish[] Jellies;


    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.tag=="Player")
        {
            for (int i = 0; i < Jellies.Length; i++)
                Jellies[i].OnPlayerHit(c.GetComponent<Player>(), null);
        }

        StartCoroutine(KillPlayer(c.GetComponent<Player>()));
    }

    IEnumerator KillPlayer(Player p)
    {
        p.SetDead(true);
        while(p.Body.velocity.magnitude>1)
        {
            Vector2 v = p.Body.velocity;
            v *= Mathf.Min(57 * Time.deltaTime, 0.95f);
            p.Body.velocity = v;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < Jellies.Length; i++)
            Jellies[i].Kill();

        p.Kill("carl_dieJelly", new Vector3());
    }

}

