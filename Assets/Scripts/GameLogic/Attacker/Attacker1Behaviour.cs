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
        
        if(gameManager.multiplayerPhase != MultiplayerPhase.InGame && gameManager.singleplayerPhase != SingleplayerPhase.InGame)
            return;
        
        
        InstBullet(transform.forward);
    }
    
    public override void Start()
    {
        base.Start();
        if(photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), ShootingDelay, ShootingSpeed);
        }
        
        cost = 100;
    }
}
