using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;

public class WeaponDropBehaviour : PhotonDropBehaviour
{
    public override void Start()
    {
        base.Start();
        type = DropType.Weapon;
        value = 20;
    }
}
