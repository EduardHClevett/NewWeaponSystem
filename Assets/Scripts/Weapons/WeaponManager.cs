using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject primaryWeapon, secondaryWeapon;

    GameObject currentWeapon;

    [Space, SerializeField]List<Weapon_Base> weapons = new List<Weapon_Base>();

    PlayerInputs inputs;

    private void Awake()
    {
        inputs = new PlayerInputs();

        inputs.InGame.SwapWeapons.performed += _ => SwapWeapon();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }


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
            { primaryWeapon = wep.gameObject; }
            else if (secondaryWeapon == null && wep.isEquipped && wep != primaryWeapon)
            { secondaryWeapon = wep.gameObject; break; }

            else
                wep.gameObject.SetActive(false);
        }

        secondaryWeapon.SetActive(false);

        currentWeapon = primaryWeapon;
    }

    void SwapWeapon()
    {
        if (currentWeapon = primaryWeapon)
        {
            currentWeapon.SetActive(false);
            currentWeapon = secondaryWeapon;
            currentWeapon.SetActive(true);
            return;
        }
        
        if (currentWeapon = secondaryWeapon)
        {
            currentWeapon.SetActive(false);
            currentWeapon = primaryWeapon;
            currentWeapon.SetActive(true);
            return;
        }
    }
}
