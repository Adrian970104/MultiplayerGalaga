using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Attacker2Behaviour : AttackerShipBehaviour
{
    public override void Shooting()
    {
        if (!isDeployed)
            return;
        
        if (gameManager.multiplayerPhase != MultiplayerPhase.InGame)
            return;

        var position = transform.position;
        var dir = Vector3.Normalize(defenderShip.transform.position - position);
        var rotation = Quaternion.FromToRotation(transform.forward, dir).eulerAngles;
        rotation.x += 90;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, position, Quaternion.Euler(rotation), 0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        bulletBehav.selfDirection = dir;
        bulletBehav.ownerTag = gameObject.tag;
    }

    public override void Start()
    {
        base.Start();
        if (photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(0.5f, 1.5f));
        }
        Debug.Log($"trigger count: {triggerCount}");
    }
}
