using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmo
{
    int currentMagAmmo { get; set; }
    int maxMagAmmo { get; set; }

    int currentReserveAmmo { get; set; }
    int maxReserveAmmo { get; set; }
}