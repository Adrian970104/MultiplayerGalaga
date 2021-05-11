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
            InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(1f, 2f));
        }
        Debug.Log($"trigger count: {triggerCount}");
    }
}
