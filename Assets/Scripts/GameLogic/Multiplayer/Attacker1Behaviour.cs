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
        
        //TODO Csak akkor kezdjen el lőni, ha már a deoploy phase véget ért.
        if(gameManager.multiplayerPhase != MultiplayerPhase.InGame)
            return;
        
        //var dir = Vector3.Normalize(defenderShip.transform.position - transform.position);
        //var rotation = Quaternion.FromToRotation(transform.forward, dir).eulerAngles;
        //rotation.x += 90;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(90,0,0),0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        //TODO Egyenes lüvés lefelé:
        bulletBehav.selfDirection = -1*transform.forward;
        //bulletBehav.selfDirection = dir;
        bulletBehav.ownerTag = gameObject.tag;
    }
    
    public override void Start()
    {
        base.Start();
        if(photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(0.5f, 1.5f));
        }
        Debug.Log($"trigger count: {triggerCount}");
    }
}
