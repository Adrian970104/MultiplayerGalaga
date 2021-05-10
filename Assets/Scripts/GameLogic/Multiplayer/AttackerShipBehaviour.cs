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


    /*public PhotonBulletBehaviour InstBullet(Vector3 dir)
    {
        var rotation = Quaternion.FromToRotation(transform.forward, dir).eulerAngles;
        rotation.x += 90;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(rotation), 0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        bulletBehav.selfDirection = dir;
        bulletBehav.ownerTag = gameObject.tag;
        return bulletBehav;
    }*/
    
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
        _feedbackPanelController = FindObjectOfType<MultiplayerFeedbackPanelController>();
        
        if(photonView.IsMine)
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
            attackerPlayer.EndCheck();
            defenderShip.GetComponent<DefenderShipBehaviour>().Defeated();
        }
        
        if (other.CompareTag("AttackerShip") || other.CompareTag("SpawnSphere"))
        {
            Debug.Log($"Other ship or Spawn point entered");
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
        GetComponent<Renderer>().material.SetColor($"_Color", Color.cyan);
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
