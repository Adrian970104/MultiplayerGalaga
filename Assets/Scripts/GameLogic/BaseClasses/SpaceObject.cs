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
    
    protected Vector3 _selfPos;
    protected Vector3 _starPos;
    protected static int _maxDistance = 50;
    protected float _lag;
    
    
    protected void Movement(Vector3 direction)
    {
        transform.position += direction * (speed * Time.deltaTime);
    }
    
    protected void DestroyCheck()
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
            _lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
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
            transform.position = Vector3.Lerp(transform.position, _selfPos, Time.deltaTime*10 + _lag);
        }
    }
    #endregion
}
