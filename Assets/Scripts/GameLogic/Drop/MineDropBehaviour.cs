using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Photon.Pun;
using UnityEngine;

public class MineDropBehaviour : DropBehaviour
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
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy && _gameManager.multiplayerPhase != MultiplayerPhase.InAttack && _gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
            return;

        var defenderShip = other.GetComponentInParent<DefenderShipBehaviour>();
        defenderShip.photonView.RPC("TakeDamage", RpcTarget.All, value);
        defenderShip.photonView.RPC("ResetDamage", RpcTarget.All);
        defenderShip.photonView.RPC("ResetHealth", RpcTarget.All);
        
        if(photonView.IsMine)
            PhotonNetwork.Destroy(photonView);
    }
}