using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class SpaceShip : MonoBehaviour, IPunObservable
{
    public GameObject bullet;
    public PhotonView photonView;
    public Vector3 selfPos;
    public int health;
    public int damage;

    
    public void TakeDamage(int dam)
    {
        health -= dam;
        HealthCheck();
    }

    private void HealthCheck()
    {
        if (health > 0) return;
        gameObject.tag = "Untagged";
        photonView.RPC("RPCDestroy", RpcTarget.All);
    }
    
    [PunRPC]
    public void RPCDestroy()
    {
        if (!photonView.IsMine) return;
        PhotonNetwork.Destroy(gameObject);
    }
    
    public virtual void Shooting()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
