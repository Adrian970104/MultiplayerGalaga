using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackerShipBehaviour : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    public Vector3 selfDirection;

    public GameObject bullet;
    public int speed = 1;
    public bool isDeployed;

    private Vector3 _selfPos;
    private int _health = 100;



    private void Shooting()
    { 
        if(!isDeployed) return;

        //var _rigidbody = gameObject.GetComponent<Rigidbody>();
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(90,0,0),0);
        //bulletClone.GetComponent<PhotonBulletBehaviour>().selfDirection = -1*transform.forward;
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        bulletBehav.selfDirection = -1*transform.forward;
        //bulletBehav.photonView.RPC("SetOwnerTag",RpcTarget.All,gameObject.tag);
        bulletBehav.ownerTag = gameObject.tag;
    }
    public void Movement(Vector3 direction)
    {
        //transform.position += direction * (speed * Time.deltaTime);
    }
    
    [PunRPC]
    public void RPCDestroy()
    {
        if (!photonView.IsMine) return;
        PhotonNetwork.Destroy(gameObject);
    }

    public void ChangeHealth(int amount)
    {
        _health += amount;
        HealthCheck();
    }

    private void HealthCheck()
    {
        if (_health > 0) return;
        gameObject.tag = "Untagged";
        photonView.RPC("RPCDestroy", RpcTarget.All);
    }

    #region Photon Methods

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            _selfPos = (Vector3)stream.ReceiveNext();
        }
    }
    #endregion
        
    #region Unity Methods
    private void Update()
    {
        if (photonView.IsMine)
        {
            Movement(selfDirection);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _selfPos, Time.deltaTime * 15);
        }

        if (isDeployed)
        {
            
        }
    }

    private void Start()
    {
        _selfPos = transform.position;
        isDeployed = false;
        InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(0.1f, 0.5f));
    }

    #endregion
}
