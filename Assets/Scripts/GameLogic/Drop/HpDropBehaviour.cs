using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Photon.Pun;
using UnityEngine;

public class HpDropBehaviour : PhotonDropBehaviour
{
    public override void Start()
    {
        base.Start();
        type = DropType.Hp;
        value = 20;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!photonView.IsMine)
            return;
        if(!other.CompareTag("DefenderShip"))
            return;
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy && _gameManager.multiplayerPhase != MultiplayerPhase.InAttack && _gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
            return;

        other.gameObject.GetComponentInParent<DefenderShipBehaviour>().photonView.RPC("Heal", RpcTarget.All, value);
        
        if(photonView.IsMine)
            PhotonNetwork.Destroy(photonView);
    }
}
