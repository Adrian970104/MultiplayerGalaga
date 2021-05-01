using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonDefenderBehaviour : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    public GameObject bullet;

    private Vector3 _selfPos;
    private Rigidbody _rigidbody;
    private bool _isAttacker;

    private readonly int _leftBorder = -28;
    private readonly int _rightBorder = 28;
    private readonly int _upBorder = -13;
    private readonly int _downBorder = -19;

    private void AddForceMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.AddForce(transform.right * (2000 * Time.deltaTime));
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.AddForce(transform.right * (-2000 * Time.deltaTime));
        }
            
        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.AddForce(transform.forward * (2000 * Time.deltaTime));
        }
            
        if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.AddForce(transform.forward * (-2000 * Time.deltaTime));
        }
    }

    private void BorderCheck()
    {
        if (transform.position.x > _rightBorder)
        {
            _rigidbody.velocity = new Vector3(0,0,0);
            transform.position = new Vector3(_rightBorder,transform.position.y,transform.position.z);
        }
        
        if (transform.position.x < _leftBorder)
        {
            _rigidbody.velocity = new Vector3(0, 0, 0);
            transform.position = new Vector3(_leftBorder, transform.position.y, transform.position.z);
        }
        
        if (transform.position.z > _upBorder)
        {
            _rigidbody.velocity = new Vector3(0, 0, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y, _upBorder);
        }
        
        if (transform.position.z < _downBorder)
        {
            _rigidbody.velocity = new Vector3(0,0,0);
            transform.position = new Vector3(transform.position.x,transform.position.y,_downBorder);
        }
    }

    private void Shooting()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, _rigidbody.position, Quaternion.Euler(90,0,0),0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        bulletBehav.selfDirection = transform.forward;
        bulletBehav.ownerTag = gameObject.tag;
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
            _rigidbody.position = Vector3.Lerp(_rigidbody.position, _selfPos, Time.fixedDeltaTime * 12);
        }
    }

    private void Update()
    {
        if (_isAttacker) return;
        AddForceMovement();
        BorderCheck();
        Shooting();
    }
    
    #endregion

}
