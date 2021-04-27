using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonPlayerBehaviour : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    public GameObject bullet;
    public PhotonView photonView;
    private Vector3 _selfPos;
    private Vector3 _selfVel;
    private Rigidbody _rigidbody;
    private float _lag;

    private void AddForceMovement()
    {
        var rbody = GetComponent<Rigidbody>();
        if (Input.GetKey(KeyCode.D))
        {
            rbody.AddForce(transform.right * (2000 * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.A))
        {
            rbody.AddForce(transform.right * (-2000 * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.W))
        {
            rbody.AddForce(transform.forward * (2000 * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S))
        {
            rbody.AddForce(transform.forward * (-2000 * Time.deltaTime));
        }
    }

    private void Shooting()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, _rigidbody.position, Quaternion.Euler(90,0,0),0);
        bulletClone.GetComponent<PhotonBulletBehaviour>().selfDirection = transform.forward;
    }

    #region Photon Methods

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rigidbody.position);
            //stream.SendNext(_rigidbody.velocity);
        }
        else
        {
            _selfPos = (Vector3)stream.ReceiveNext();
            //_selfVel = (Vector3)stream.ReceiveNext();
            
            //_lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
           /*_rigidbody.position = (Vector3) stream.ReceiveNext();
           _rigidbody.velocity = (Vector3) stream.ReceiveNext();

           float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
           _rigidbody.position += _rigidbody.velocity * lag;*/
        }
    }
    #endregion
    
    
    #region Unity Methods
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
        {
            _rigidbody.position = Vector3.Lerp(_rigidbody.position, _selfPos, Time.fixedDeltaTime * 15);
            //_rigidbody.position = Vector3.MoveTowards(_rigidbody.position, networkPosition, Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        if (!(bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
        {
            AddForceMovement();
            Shooting();
        }
        else
        {
            //_rigidbody.velocity = _selfVel;
            //_rigidbody.position += _rigidbody.velocity * _lag / _rigidbody.drag ;
        }
    }
    #endregion

}
