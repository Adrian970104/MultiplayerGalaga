using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class SpaceShip : MonoBehaviour, IPunObservable
{
    public GameObject bullet;
    public PhotonView photonView;
    public Vector3 selfPos;
    public int maxHealth;
    public int actualHealth;
    public int baseDamage;
    public int actualDamage;
    
    public virtual void IncreaseDamage(int amount)
    {
        actualDamage += amount;
    }
    
    public virtual void ResetDamage()
    {
        actualDamage = baseDamage;
    }
    
    public virtual void Heal(int amount)
    {
        var healed = actualHealth + amount;
        actualHealth = healed >= maxHealth ? maxHealth : healed;
    }
    
    [PunRPC]
    public virtual void TakeDamage(int dam)
    {
        actualHealth -= dam;
        HealthCheck();
    }

    protected virtual void HealthCheck()
    {
        if (actualHealth > 0)
            return;
        gameObject.tag = "Untagged";
        photonView.RPC("RPCDestroy", RpcTarget.All);
    }
    
    public BulletBehaviour InstBullet(Vector3 dir)
    {
        var rotation = Quaternion.FromToRotation(transform.forward, dir).eulerAngles;
        rotation.x += 90;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(rotation), 0);
        var bulletBehav = bulletClone.GetComponent<BulletBehaviour>();
        bulletBehav.selfDirection = dir;
        bulletBehav.photonView.RPC("SetOwnerTag",RpcTarget.All,gameObject.tag);
        //bulletBehav.ownerTag = gameObject.tag;
        return bulletBehav;
    }
    
    [PunRPC]
    public void RPCDestroy()
    {
        if (!photonView.IsMine)
            return;
        PhotonNetwork.Destroy(gameObject);
    }

    public abstract void Shooting();

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
        }
    }
}
