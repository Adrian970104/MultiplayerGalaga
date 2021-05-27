using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Attacker1Behaviour : AttackerShipBehaviour
{
    public override void Shooting()
    {
        if(!isDeployed) 
            return;
        
        if(gameManager.multiplayerPhase != MultiplayerPhase.InAttack && gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
            return;

        InstBullet(transform.forward);
    }

    [PunRPC]
    public override void TakeDamage(int dam)
    {
        Shooting();
        base.TakeDamage(dam);
    }

    public override void Start()
    {
        base.Start();
        if(photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), ShootingDelay, ShootingSpeed);
        }
        
        cost = 100;
        value = 30;
    }
}
