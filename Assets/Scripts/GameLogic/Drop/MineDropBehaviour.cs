using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;

public class MineDropBehaviour : PhotonDropBehaviour
{

    public override void Start()
    {
        base.Start();
        type = DropType.Mine;
        value = 150;
    }
}
