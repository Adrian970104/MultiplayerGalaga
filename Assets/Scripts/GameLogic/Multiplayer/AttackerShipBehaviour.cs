using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackerShipBehaviour : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    public int speed = 1;
    public Vector3 selfDirection;

    private Vector3 _selfPos;
    private int _health = 100;

        
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
    
    private void OnTriggerEnter(Collider other)
    {
        /*if (other.CompareTag("Bullet"))
        {
            PhotonNetwork.Destroy(gameObject);
        }*/
    }

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
    }

    private void Start()
    {
        _selfPos = transform.position;
    }

    #endregion
}
