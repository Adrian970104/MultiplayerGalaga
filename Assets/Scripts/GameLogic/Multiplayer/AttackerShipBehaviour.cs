using System;
using Photon.Pun;
using UnityEngine;

public class AttackerShipBehaviour : SpaceShip, IPunObservable
{
#region deployment_variables
    public bool isDeployed;
    public int triggerCount;
    private Color _baseColor;
#endregion

    private MultiplayerFeedbackPanelController _feedbackPanelController;
    public GameManager gameManager;
    public PhotonAttackerBehaviour attackerPlayer;
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
        //TODO Adrian Ez így nem túl szép!
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
        health = 150;
        damage = 50;
        
        gameManager = FindObjectOfType<GameManager>();
        defenderShip = GameObject.FindGameObjectWithTag("DefenderShip");
        selfPos = transform.position;
        _baseColor = GetComponent<Renderer>().material.color;
        _feedbackPanelController = FindObjectOfType<MultiplayerFeedbackPanelController>();
        
        isDeployed = gameManager.gameMode == GameMode.Singleplayer;
        
        if(photonView.IsMine && gameManager.gameMode == GameMode.Multiplayer)
        {
            GetComponent<Renderer>().material.SetColor($"_Color", Color.red);
        }
    }


    protected void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;
        
        if (other.CompareTag("DefenderShip"))
        {
            Debug.Log("Entered the Defender ship");
            if(gameManager.multiplayerPhase != MultiplayerPhase.InGame)
                return;
            
            gameObject.tag = "Untagged";
            photonView.RPC("RPCDestroy", RpcTarget.All);
            defenderShip.GetComponent<DefenderShipBehaviour>().Defeated();
        }
        
        if (other.CompareTag("AttackerShip") || other.CompareTag("SpawnSphere"))
        {
            if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;
            
            if(isDeployed)
                return;
            
            triggerCount++;
            GetComponent<Renderer>().material.SetColor($"_Color", Color.red);
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
            if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
                return;

            --triggerCount;
            if(triggerCount > 0) 
                return;
            
            GetComponent<Renderer>().material.SetColor($"_Color", _baseColor);
        }
    }

    private bool MouseInteractable()
    {
        if(!isDeployed)
            return false;
        if(gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return false;
        
        return (bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"];
    }

    private void OnMouseEnter()
    {
        if(!MouseInteractable())
            return;
        GetComponent<Renderer>().material.SetColor($"_Color", Color.black);
    }

    private void OnMouseExit()
    {
        if(!MouseInteractable())
            return;
        GetComponent<Renderer>().material.SetColor($"_Color", _baseColor);
        
    }

    private void OnMouseUp()
    {
        if(!MouseInteractable())
            return;
        PhotonNetwork.Destroy(gameObject);
        attackerPlayer.shipCount++;
        _feedbackPanelController.RefreshShipCountText();
        attackerPlayer.attackerShips.Remove(this);
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine) 
            return;
        if(attackerPlayer is null)
            return;
        if(!attackerPlayer.attackerShips.Contains(this))
            return;
            
        attackerPlayer.attackerShips.Remove(this);
        
        if(gameManager.multiplayerPhase == MultiplayerPhase.InGame)
        {
            attackerPlayer.EndCheck();
        }
    }

    #endregion
}
