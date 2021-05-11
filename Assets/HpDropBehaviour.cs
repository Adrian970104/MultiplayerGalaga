using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;

public class HpDropBehaviour : PhotonDropBehaviour
{
    public override void Start()
    {
        base.Start();
        type = DropType.Hp;
        value = 50;
    }
}
