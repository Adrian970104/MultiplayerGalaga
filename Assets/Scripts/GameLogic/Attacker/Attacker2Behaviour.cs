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
        
        if (gameManager.multiplayerPhase != MultiplayerPhase.InAttack && gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
            return;
        
        var dir = Vector3.Normalize(defenderShip.transform.position - transform.position);
        InstBullet(dir);
    }

    public override void Start()
    {
        base.Start();
        if (photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), ShootingDelay, ShootingSpeed);
        }
        cost = 200;
    }
}
