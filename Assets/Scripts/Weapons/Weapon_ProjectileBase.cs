using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_ProjectileBase : Weapon_Base
{
    public GameObject projectile;

    protected override void EnterFiring()
    {
        base.EnterFiring();

        isFiring = true;
    }

    IEnumerator Fire(float repeats = 1)
    {
        while(true)
        {
            if (fireTimer < (1 / fireRate)) fireTimer += Time.deltaTime;

            if(isFiring)
            {

            }
        }
    }
}
