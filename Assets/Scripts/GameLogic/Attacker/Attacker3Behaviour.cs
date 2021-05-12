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

        for (var i = 1; i <= 6; i++)
        {
            var degrees = i * 60;
            var dir = Quaternion.Euler(0, degrees, 0) * transform.forward;
            InstBullet(dir);
        }
    }
    public override void Start()
    {
        base.Start();
        if(photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(2f, 4f));
        }
        cost = 300;
    }
}
