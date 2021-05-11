using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Photon.Pun;
using UnityEngine;

public class PhotonDropBehaviour : MonoBehaviour, IPunObservable
{
    public DropType type;
    public int value;
    
    public PhotonView photonView;
    public int speed = 2;
    public Vector3 selfDirection;
    private Vector3 _selfPos;
    private Quaternion _selfRot;
    private float _lag;
    
    private void Movement(Vector3 direction)
    {
        transform.position += direction * (speed * Time.deltaTime);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _selfPos = (Vector3) stream.ReceiveNext();
            _selfRot = (Quaternion) stream.ReceiveNext();
            _lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
        }
    }
    #region Unity methods
    
    void Update()
    {
        transform.Rotate(new Vector3(0,0.2f,0));
        if (photonView.IsMine)
        {
            Movement(selfDirection);
            //DestroyCheck();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _selfPos, Time.deltaTime*10);
            transform.rotation = _selfRot;
        }
    }

    public virtual void Start()
    {
        _selfPos = transform.position;
        _selfRot = transform.rotation;
    }

    #endregion
}
