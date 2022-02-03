using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Stats", menuName = "Scriptable Objects/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public float damage;
    public float maxMagCount;
    public float maxReserveCount;
    public float fireRate;

    public bool usesAmmo = true;
}
