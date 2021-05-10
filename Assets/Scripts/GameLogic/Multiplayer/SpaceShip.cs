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

    protected virtual void HealthCheck()
    {
        if (health > 0) return;
        gameObject.tag = "Untagged";
        photonView.RPC("RPCDestroy", RpcTarget.All);
    }
    
    public PhotonBulletBehaviour InstBullet(Vector3 dir)
    {
        var rotation = Quaternion.FromToRotation(transform.forward, dir).eulerAngles;
        rotation.x += 90;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(rotation), 0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        bulletBehav.selfDirection = dir;
        bulletBehav.ownerTag = gameObject.tag;
        return bulletBehav;
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
