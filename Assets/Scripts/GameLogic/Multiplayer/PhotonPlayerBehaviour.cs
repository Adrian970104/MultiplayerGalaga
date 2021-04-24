using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonPlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    public GameObject bullet;
    public PhotonView photonView;
    private Vector3 _selfPos;

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
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(90,0,0),0);
        //bulletClone.transform.Rotate(new Vector3(1, 0, 0), 90);
        bulletClone.GetComponent<PhotonBulletBehaviour>().selfDirection = transform.forward;
    }
    
    

    #region Photon Methods
    

    #endregion
    
    
    #region Unity Methods
    void Start()
    {
    }
    
    void FixedUpdate()
    {
        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
        {
            AddForceMovement();
        }
    }

    private void Update()
    {
        if (!(bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
        {
            Shooting();
        }
    }

    #endregion
}
