using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpaceObject : MonoBehaviour, IPunObservable
{
    public int speed;
    public PhotonView photonView;
    public Vector3 selfDirection;

    private Vector3 _selfPos;
    private Vector3 _starPos;
    private static int _maxDistance = 160;

    private void Movement(Vector3 direction)
    {
        transform.position += direction * (speed * Time.deltaTime);
    }

    private void DestroyCheck()
    {
        if (Math.Abs(Vector3.Distance(transform.position, _starPos)) > _maxDistance)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
    
    #region Photon Methods

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            _selfPos = (Vector3) stream.ReceiveNext();
        }
    }
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    public virtual void Start()
    {
        _selfPos = transform.position;
        _starPos = _selfPos;
    }

    public virtual void Update()
    {
        if (photonView.IsMine)
        {
            Movement(selfDirection);
            DestroyCheck();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _selfPos, Time.deltaTime * 11);
        }
    }
    #endregion
}
