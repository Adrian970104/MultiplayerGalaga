﻿using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DefenderShipBehaviour : SpaceShip
{
    private Rigidbody _rigidbody;
    private bool _isAttacker;

    private readonly int _force = 1500;
    private readonly int _leftBorder = -28;
    private readonly int _rightBorder = 16;
    private readonly int _upBorder = -10;
    private readonly int _downBorder = -18;
    private GameManager _gameManager;
    private MultiplayerInGameManager _multiManager;
    private MultiplayerDefenderCanvasController _canvasController;

    private void AddForceMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.AddForce(transform.right * (_force * Time.deltaTime));
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.AddForce(transform.right * (-_force * Time.deltaTime));
        }
            
        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.AddForce(transform.forward * (_force * Time.deltaTime));
        }
            
        if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.AddForce(transform.forward * (-_force * Time.deltaTime));
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

    public override void Shooting()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) 
            return;
        
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InGame && _gameManager.singleplayerPhase != SingleplayerPhase.InGame)
            return;
        
        InstBullet(transform.forward);
    }

    [PunRPC]
    public override void ResetDamage()
    {
        base.ResetDamage();
        if(!photonView.IsMine)
            return;
        _canvasController.RefreshDataPanel(actualDamage);
    }

    [PunRPC]
    public override void IncreaseDamage(int amount)
    {
        base.IncreaseDamage(amount);
        if(!photonView.IsMine)
            return;
        _canvasController.RefreshDataPanel(actualDamage);
    }

    [PunRPC]
    public override void Heal(int amount)
    {
        base.Heal(amount);
        Debug.Log($"Healed with {amount} hp");
        if(!photonView.IsMine)
            return;
        _canvasController.RefreshHealthPanel(actualHealth,maxHealth);
    }

    [PunRPC]
    public override void TakeDamage(int dam)
    {
        base.TakeDamage(dam);
        Debug.Log($"Damage taken {dam}");
        if(!photonView.IsMine)
            return;
        _canvasController.RefreshHealthPanel(actualHealth,maxHealth);
    }

    protected override void HealthCheck()
    {
        base.HealthCheck();
        if (actualHealth > 0)
            return;
        Defeated();
    }

    public void Defeated()
    {
        if (_gameManager.gameMode == GameMode.Multiplayer)
        {
            var winner = _isAttacker ? PhotonNetwork.LocalPlayer.NickName : PhotonNetwork.PlayerListOthers[0].NickName;
            _multiManager.photonView.RPC("EndMultiplayer", RpcTarget.All, winner);
        }

        if (_gameManager.gameMode == GameMode.Singleplayer)
        {
            FindObjectOfType<SingleplayerInGameManager>().EndSingleplayer(false);
        }
    }

    #region Photon Methods

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rigidbody.position);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
        }
    }
    #endregion
    
    
    #region Unity Methods
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();
        
        maxHealth = 300;
        actualHealth = maxHealth;
        baseDamage = 50;
        actualDamage = baseDamage;
        
        if (_gameManager.gameMode == GameMode.Multiplayer)
        {
            _isAttacker = (bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"];
            _multiManager = FindObjectOfType<MultiplayerInGameManager>();

            if (photonView.IsMine)
            {
                _canvasController = FindObjectOfType<MultiplayerDefenderCanvasController>();
                _canvasController.RefreshHealthPanel(actualHealth,maxHealth);
                _canvasController.RefreshDataPanel(actualDamage);
            }
        }
        else
        {
            _isAttacker = false;
        }

    }

    private void FixedUpdate()
    {
        if (_isAttacker)
        {
            _rigidbody.position = Vector3.Lerp(_rigidbody.position, selfPos, Time.fixedDeltaTime * 12);
        }
    }

    private void Update()
    {
        if (_isAttacker)
            return;
        AddForceMovement();
        BorderCheck();
        Shooting();
    }
    
    #endregion

}