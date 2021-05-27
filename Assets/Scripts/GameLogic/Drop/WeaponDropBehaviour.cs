using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Photon.Pun;
using UnityEngine;

public class WeaponDropBehaviour : DropBehaviour
{
    public override void Start()
    {
        base.Start();
        type = DropType.Weapon;
        value = 10;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if(!photonView.IsMine)
            return;
        if(!other.CompareTag("DefenderShip"))
            return;
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy && _gameManager.multiplayerPhase != MultiplayerPhase.InAttack && _gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
            return;

        other.gameObject.GetComponentInParent<DefenderShipBehaviour>().photonView.RPC("IncreaseDamage", RpcTarget.All, value);
        
        if(photonView.IsMine)
            PhotonNetwork.Destroy(photonView);
    }
}