using System;
using System.Collections.Generic;
using GameLogic;
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

    public List<GameObject> Drops = new List<GameObject>();
    
    [SerializeField]
    private int _dropChance = 90;
    private MultiplayerFeedbackPanelController _feedbackPanelController;

    public int cost;
    public int value;
    public SingleplayerInGameManager singleManager;
    public GameManager gameManager;
    public PhotonAttackerBehaviour attackerPlayer;
    public GameObject defenderShip;
    public ParticleSystem explosion;


    public void Drop()
    {
        var isDropping = _dropChance >= Random.Range(-1, 100) ? true : false;
        Debug.Log($"Dropping is : {isDropping}");
        if(!isDropping)
            return;
        
        var index = Random.Range(0, Drops.Count);
        var chosenDrop = Drops[index];
        var drop = PhotonNetwork.Instantiate(chosenDrop.name, transform.position, Quaternion.identity);
        drop.GetComponent<PhotonDropBehaviour>().selfDirection = transform.forward;
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
        maxHealth = 150;
        baseDamage = 40;
        actualDamage = baseDamage;
        
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
        attackerPlayer.material += cost;
        _feedbackPanelController.RefreshFeedbackPanel();
        attackerPlayer.attackerShips.Remove(this);
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine) 
            return;
        
        switch (gameManager.gameMode)
        {
            case GameMode.Singleplayer:
            {
                if(gameManager.singleplayerPhase != SingleplayerPhase.InGame)
                    return;
                
                if (singleManager.attackerShips.Contains(this))
                    singleManager.attackerShips.Remove(this);
                
                Drop();
                
                defenderShip.GetComponentInParent<DefenderShipBehaviour>().AddScore(value);
                
                singleManager.EndCheck();
                break;
            }
            case GameMode.Multiplayer:
            {
                if (attackerPlayer is null)
                    return;

                if (!attackerPlayer.attackerShips.Contains(this))
                    return;

                attackerPlayer.attackerShips.Remove(this);

                if (gameManager.multiplayerPhase == MultiplayerPhase.InGame)
                {
                    Drop();
                    attackerPlayer.WaveEndCheck();
                }
                break;
            }
            case GameMode.Undefined:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
    }

    #endregion
}
