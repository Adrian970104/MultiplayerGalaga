using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackerShipBehaviour : SpaceShip, IPunObservable
{
#region deployment_variables
    public bool isDeployed;
    public int triggerCount = 1;
    private Color _baseColor;
#endregion
    private GameObject _defenderShip;
    private GameManager _gameManager;

    public override void Shooting()
    {
        if(!isDeployed) 
            return;
        
        //TODO Csak akkor kezdjen el lőni, ha már a deoploy phase véget ért.
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InGame)
            return;
        
        var dir = Vector3.Normalize(_defenderShip.transform.position - transform.position);
        var rotation = Quaternion.FromToRotation(transform.forward, dir).eulerAngles;
        rotation.x += 90;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(rotation),0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        //TODO Egyenes lüvés lefelé:
        //bulletBehav.selfDirection = -1*transform.forward;
        bulletBehav.selfDirection = dir;
        bulletBehav.ownerTag = gameObject.tag;
    }
    
    #region Photon Methods

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(isDeployed);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
            isDeployed = (bool)stream.ReceiveNext();
        }
    }
    #endregion
        
    #region Unity Methods
    private void Update()
    {
        if (photonView.IsMine)
        {
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, selfPos, Time.deltaTime * 15);
        }
    }

    private void Start()
    {
        health = 100;
        damage = 50;
        
        _gameManager = FindObjectOfType<GameManager>();
        _defenderShip = GameObject.FindGameObjectWithTag("DefenderShip");
        selfPos = transform.position;
        _baseColor = GetComponent<Renderer>().material.color;
        isDeployed = false;
        
        if(photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(0.5f, 1.5f));
            GetComponent<Renderer>().material.SetColor($"_Color", Color.blue);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;
        
        if (other.CompareTag("DefenderShip"))
        {
            //TODO Itt A játék is véget ér, a defender hajó azonnal meghal!.
            Debug.Log("Entered the Defender ship");
            //RPCDestroy();
        }
        
        if (other.CompareTag("AttackerShip") || other.CompareTag("SpawnSphere"))
        {
            Debug.Log("Entered Attacker ship");
            if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;
            
            if(isDeployed)
                return;
            
            triggerCount++;
            GetComponent<Renderer>().material.SetColor($"_Color", Color.blue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (!photonView.IsMine)
            return;
        
        if (other.CompareTag("SpawnSphere"))
        {
            if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;
            --triggerCount;
            GetComponent<Renderer>().material.SetColor($"_Color", _baseColor);
        }

        if (other.CompareTag("AttackerShip"))
        {
            Debug.Log("Exited Attacker ship");
            if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;

            --triggerCount;
            if(triggerCount > 0) 
                return;
            
            GetComponent<Renderer>().material.SetColor($"_Color", _baseColor);
        }
    }

    #endregion
}
