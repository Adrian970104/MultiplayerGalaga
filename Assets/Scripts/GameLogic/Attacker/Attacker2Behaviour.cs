using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Attacker2Behaviour : AttackerShipBehaviour
{
    public override void Shooting()
    {
        if (!isDeployed)
            return;
        
        if (gameManager.multiplayerPhase != MultiplayerPhase.InGame && gameManager.singleplayerPhase != SingleplayerPhase.InGame)
            return;
        
        var dir = Vector3.Normalize(defenderShip.transform.position - transform.position);
        InstBullet(dir);
    }

    public override void Start()
    {
        base.Start();
        if (photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(1.5f, 2.5f));
        }
        Debug.Log($"trigger count: {triggerCount}");
    }
}
