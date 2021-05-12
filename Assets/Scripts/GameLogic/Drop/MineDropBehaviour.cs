using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Photon.Pun;
using UnityEngine;

public class MineDropBehaviour : PhotonDropBehaviour
{

    public override void Start()
    {
        base.Start();
        type = DropType.Mine;
        value = 150;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if(!photonView.IsMine)
            return;
        if(!other.CompareTag("DefenderShip"))
            return;
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InGame && _gameManager.singleplayerPhase != SingleplayerPhase.InGame)
            return;

        other.gameObject.GetComponentInParent<DefenderShipBehaviour>().photonView.RPC("ResetDamage", RpcTarget.All);
        other.gameObject.GetComponentInParent<DefenderShipBehaviour>().photonView.RPC("TakeDamage", RpcTarget.All, value);
        
        if(photonView.IsMine)
            PhotonNetwork.Destroy(photonView);
    }
}
