using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackerShipBehaviour : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    public Vector3 selfDirection;
    public int triggerCount = 0;

    public GameObject bullet;
    public int speed = 1;
    public bool isDeployed;

    private Vector3 _selfPos;
    private int _health = 100;
    private GameObject _defenderShip;
    private GameManager _gameManager;
    private Color _baseColor;



    private void Shooting()
    {
        if(!isDeployed) 
            return;
        
        //TODO Csak akkor kezdjen el lőni, ha már a deoploy phase véget ért.
        //if(_gameManager.multiplayerPhase != MultiplayerPhase.InGame) return;
        
        var dir = Vector3.Normalize(_defenderShip.transform.position - transform.position);
        var rotation = Quaternion.FromToRotation(transform.forward, dir).eulerAngles;
        rotation.x += 90;
        var bulletClone = PhotonNetwork.Instantiate(bullet.name, transform.position, Quaternion.Euler(rotation),0);
        var bulletBehav = bulletClone.GetComponent<PhotonBulletBehaviour>();
        //bulletBehav.selfDirection = -1*transform.forward;
        bulletBehav.selfDirection = dir;
        bulletBehav.ownerTag = gameObject.tag;
    }
    public void Movement(Vector3 direction)
    {
        //transform.position += direction * (speed * Time.deltaTime);
    }
    
    [PunRPC]
    public void RPCDestroy()
    {
        if (!photonView.IsMine) return;
        PhotonNetwork.Destroy(gameObject);
    }

    public void ChangeHealth(int amount)
    {
        _health += amount;
        HealthCheck();
    }

    private void HealthCheck()
    {
        if (_health > 0) return;
        gameObject.tag = "Untagged";
        photonView.RPC("RPCDestroy", RpcTarget.All);
    }

    #region Photon Methods

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(isDeployed);
        }
        else
        {
            _selfPos = (Vector3)stream.ReceiveNext();
            isDeployed = (bool)stream.ReceiveNext();
        }
    }
    #endregion
        
    #region Unity Methods
    private void Update()
    {
        if (photonView.IsMine)
        {
            Movement(selfDirection);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _selfPos, Time.deltaTime * 15);
        }

        if (isDeployed)
        {
            
        }
    }

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _defenderShip = GameObject.FindGameObjectWithTag("DefenderShip");
        _selfPos = transform.position;
        _baseColor = GetComponent<Renderer>().material.color;
        isDeployed = false;
        if(photonView.IsMine)
        {
            InvokeRepeating(nameof(Shooting), Random.Range(2.0f, 4.0f), Random.Range(0.5f, 1.5f));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DefenderShip"))
        {
            //TODO Itt A játék is véget ér, a defender hajó azonnal meghal!.
            Debug.Log("Entered the Defender ship");
            //RPCDestroy();
        }
        
        if (other.CompareTag("AttackerShip"))
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
