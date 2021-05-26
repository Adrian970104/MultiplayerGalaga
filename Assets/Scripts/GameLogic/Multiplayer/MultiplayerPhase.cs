using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MultiplayerPhase
{
    Undefined,
    InRoomCreation,
    InRoom,
    InDeploy,
    InAttack,
    AfterGame
}