using System;
using System.Collections.Generic;
using UnityEngine;

interface IEnemy
{
    void OnPlayerHit(Player p, Collision2D c);
    void OnApplyDamage();

}

