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
    public int triggerCount;
    private Color _baseColor;
#endregion
    public GameManager gameManager;
    public GameObject defenderShip;
    
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

    public virtual void Start()
    {
        triggerCount = 0;
        health = 100;
        damage = 50;
        
        gameManager = FindObjectOfType<GameManager>();
        defenderShip = GameObject.FindGameObjectWithTag("DefenderShip");
        selfPos = transform.position;
        _baseColor = GetComponent<Renderer>().material.color;
        isDeployed = false;
        
        if(photonView.IsMine)
        {
            //InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(0.5f, 1.5f));
            GetComponent<Renderer>().material.SetColor($"_Color", Color.blue);
        }
    }


    protected void OnTriggerEnter(Collider other)
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
            if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;
            
            if(isDeployed)
                return;
            
            triggerCount++;
            GetComponent<Renderer>().material.SetColor($"_Color", Color.blue);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        
        if (!photonView.IsMine)
            return;
        
        if (other.CompareTag("SpawnSphere"))
        {
            if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;
            
            --triggerCount;
            GetComponent<Renderer>().material.SetColor($"_Color", _baseColor);
        }

        if (other.CompareTag("AttackerShip"))
        {
            Debug.Log("Exited Attacker ship");
            if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;

            --triggerCount;
            if(triggerCount > 0) 
                return;
            
            GetComponent<Renderer>().material.SetColor($"_Color", _baseColor);
        }
    }

    private void OnMouseEnter()
    {
        if(!isDeployed)
            return;
        if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        GetComponent<Renderer>().material.SetColor($"_Color", Color.cyan);
    }

    private void OnMouseExit()
    {
        if(!isDeployed)
            return;
        if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        GetComponent<Renderer>().material.SetColor($"_Color", _baseColor);
        
    }

    private void OnMouseUp()
    {
        if(!isDeployed)
            return;
        if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        PhotonNetwork.Destroy(gameObject);
        FindObjectOfType<PhotonAttackerBehaviour>().shipCount++;
    }

    #endregion
}
