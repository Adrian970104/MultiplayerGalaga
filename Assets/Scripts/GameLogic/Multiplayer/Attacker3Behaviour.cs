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
        
        if(gameManager.multiplayerPhase != MultiplayerPhase.InGame)
            return;
        
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(90,0,0),0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        bulletBehav.selfDirection = transform.forward;
        bulletBehav.ownerTag = gameObject.tag;
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
