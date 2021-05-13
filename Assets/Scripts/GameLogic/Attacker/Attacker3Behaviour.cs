using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Attacker3Behaviour : AttackerShipBehaviour
{
    public override void Shooting()
    {
        if(!isDeployed) 
            return;
        
        if(gameManager.multiplayerPhase != MultiplayerPhase.InGame && gameManager.singleplayerPhase != SingleplayerPhase.InGame)
            return;

        var n = actualHealth > (maxHealth / 3) ? 4 : 6;

        for (var i = 1; i <= n; i++)
        {
            var degrees = i * (360/n);
            var dir = Quaternion.Euler(0, degrees, 0) * transform.forward;
            InstBullet(dir);
        }
    }
    public override void Start()
    {
        base.Start();
        if(photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), ShootingDelay, ShootingSpeed);
        }
        cost = 300;
    }
}
