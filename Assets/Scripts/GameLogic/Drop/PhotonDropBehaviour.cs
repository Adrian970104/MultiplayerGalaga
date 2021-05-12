using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Photon.Pun;
using UnityEngine;

public class PhotonDropBehaviour : SpaceObject
{
    public DropType type;
    public int value;

    protected Quaternion _selfRot;
    protected Vector3 _rotSpeed = new Vector3(0,0.2f,0);
    protected GameManager _gameManager;

    
    #region Photon Methods
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            _selfRot = (Quaternion) stream.ReceiveNext();
        }
    }
    #endregion
    
    #region Unity methods
    public override void Update()
    {
        base.Update();
        
        transform.Rotate(_rotSpeed);
        
        if (!photonView.IsMine)
        {
            transform.rotation = _selfRot;
        }
    }

    public override void Start()
    {
        _selfRot = transform.rotation;
        _gameManager = FindObjectOfType<GameManager>();
    }

    #endregion
}
