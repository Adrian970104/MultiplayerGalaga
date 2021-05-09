using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MultiplayerPhase
{
    Undefined,
    InServerCreation,
    InRoom,
    InDeploy,
    InGame,
    AfterGame
}