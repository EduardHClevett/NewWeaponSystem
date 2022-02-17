using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject primaryWeapon, secondaryWeapon;

    GameObject currentWeapon;

    [Space, SerializeField]List<Weapon_Base> weapons = new List<Weapon_Base>();

    private void Start()
    {
        //Adds all children to list
        for(int i = 0; i < transform.childCount; i++)
        {
            weapons.Add(transform.GetChild(i).GetComponent<Weapon_Base>());
        }

        foreach (Weapon_Base wep in weapons)
        {
            if (primaryWeapon == null && wep.isEquipped)
            { primaryWeapon = wep.gameObject; break; }
            else if (secondaryWeapon == null && wep.isEquipped)
            { secondaryWeapon = wep.gameObject; return; }
        }
    }
}
