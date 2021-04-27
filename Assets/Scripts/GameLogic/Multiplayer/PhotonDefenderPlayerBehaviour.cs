using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonDefenderPlayerBehaviour : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    public GameObject bullet;
    public PhotonView photonView;
    
    private Vector3 _selfPos;
    private Rigidbody _rigidbody;
    private bool _isAttacker;

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
        }
        else
        {
            _selfPos = (Vector3)stream.ReceiveNext();
        }
    }
    #endregion
    
    
    #region Unity Methods
    void Start()
    {
        _isAttacker = (bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"];
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isAttacker)
        {
            _rigidbody.position = Vector3.Lerp(_rigidbody.position, _selfPos, Time.fixedDeltaTime * 15);
        }
    }

    private void Update()
    {
        if (_isAttacker) return;
        AddForceMovement();
        Shooting();
    }
    
    #endregion

}
