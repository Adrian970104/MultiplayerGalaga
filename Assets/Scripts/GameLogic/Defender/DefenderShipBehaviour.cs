using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using WebSocketSharp;

public class DefenderShipBehaviour : SpaceShip
{
    private Rigidbody _rigidbody;
    private bool _isAttacker;
    
    private GameManager _gameManager;
    private MultiplayerInGameManager _multiManager;
    private MultiplayerDefenderCanvasController _multiCanvasController;
    private SingleplayerInGameCanvasManager _singleCanvasController;
    
    private readonly int _force = 3000;
    
    private readonly int _leftBorder = -64;
    private readonly int _rightBorder = 36;
    private readonly int _upBorder = -25;
    private readonly int _downBorder = -39;

    public int score;
    public int baseHealth;
    

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
            _rigidbody.velocity = Vector3.zero;
            transform.position = new Vector3(_rightBorder,transform.position.y,transform.position.z);
        }
        
        if (transform.position.x < _leftBorder)
        {
            _rigidbody.velocity = Vector3.zero;
            transform.position = new Vector3(_leftBorder, transform.position.y, transform.position.z);
        }
        
        if (transform.position.z > _upBorder)
        {
            _rigidbody.velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, transform.position.y, _upBorder);
        }
        
        if (transform.position.z < _downBorder)
        {
            _rigidbody.velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x,transform.position.y,_downBorder);
        }
    }

    public override void Shooting()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) 
            return;
        
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InAttack && _gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
            return;
        
        var bulletc = InstBullet(transform.forward);
        
        bulletc.photonView.RPC("SetColor", RpcTarget.All, Color.green.r, Color.green.g, Color.green.b);
    }

    [PunRPC]
    public void AddScore(int amount)
    {
        score += amount;
        
        if(!photonView.IsMine)
            return;
        
        if(_gameManager.gameMode == GameMode.Multiplayer)
            _multiCanvasController.RefreshDataPanel(actualDamage, score);
            
        if(_gameManager.gameMode == GameMode.Singleplayer)
            _singleCanvasController.RefreshDataPanel(actualDamage, score);
    }
    
    [PunRPC]
    public override void ResetDamage()
    {
        base.ResetDamage();
        
        if(!photonView.IsMine)
            return;
        
        if (_gameManager.gameMode == GameMode.Multiplayer)
            _multiCanvasController.RefreshDataPanel(actualDamage, score);
        
        if(_gameManager.gameMode == GameMode.Singleplayer)
            _singleCanvasController.RefreshDataPanel(actualDamage, score);
        
    }

    [PunRPC]
    public override void IncreaseDamage(int amount)
    {
        base.IncreaseDamage(amount);
        if(!photonView.IsMine)
            return;

        if (_gameManager.gameMode == GameMode.Multiplayer)
            _multiCanvasController.RefreshDataPanel(actualDamage, score);
        
        if(_gameManager.gameMode == GameMode.Singleplayer)
            _singleCanvasController.RefreshDataPanel(actualDamage, score);
    }
    
    [PunRPC]
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        actualHealth += amount;
        
        if(!photonView.IsMine)
            return;
        
        if (_gameManager.gameMode == GameMode.Multiplayer)
            _multiCanvasController.RefreshHealthPanel(actualHealth,maxHealth);
        
        if(_gameManager.gameMode == GameMode.Singleplayer)
            _singleCanvasController.RefreshHealthPanel(actualHealth, maxHealth);
    }

    [PunRPC]
    public void ResetHealth()
    {
        maxHealth = baseHealth;
        if (actualHealth > baseHealth)
            actualHealth = baseHealth;
        
        if(!photonView.IsMine)
            return;
        
        if (_gameManager.gameMode == GameMode.Multiplayer)
            _multiCanvasController.RefreshHealthPanel(actualHealth,maxHealth);
        
        if(_gameManager.gameMode == GameMode.Singleplayer)
            _singleCanvasController.RefreshHealthPanel(actualHealth, maxHealth);
    }

    [PunRPC]
    public override void Heal(int amount)
    {
        base.Heal(amount);
        Debug.Log($"Healed with {amount} hp");
        if(!photonView.IsMine)
            return;
        
        if (_gameManager.gameMode == GameMode.Multiplayer)
            _multiCanvasController.RefreshHealthPanel(actualHealth,maxHealth);
        
        if(_gameManager.gameMode == GameMode.Singleplayer)
            _singleCanvasController.RefreshHealthPanel(actualHealth, maxHealth);
    }

    [PunRPC]
    public override void TakeDamage(int dam)
    {
        base.TakeDamage(dam);
        Debug.Log($"Damage taken {dam}");
        if(!photonView.IsMine)
            return;
        
        if (_gameManager.gameMode == GameMode.Multiplayer)
            _multiCanvasController.RefreshHealthPanel(actualHealth,maxHealth);
        
        if(_gameManager.gameMode == GameMode.Singleplayer)
            _singleCanvasController.RefreshHealthPanel(actualHealth, maxHealth);
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

    #region Unity Methods
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gameManager = FindObjectOfType<GameManager>();

        baseHealth = 300;
        maxHealth = baseHealth;
        actualHealth = baseHealth;
        baseDamage = 50;
        actualDamage = baseDamage;
        
        if (_gameManager.gameMode == GameMode.Multiplayer)
        {
            _isAttacker = (bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"];
            _multiManager = FindObjectOfType<MultiplayerInGameManager>();

            if (photonView.IsMine)
            {
                _multiCanvasController = FindObjectOfType<MultiplayerDefenderCanvasController>();
                _multiCanvasController.RefreshHealthPanel(actualHealth, maxHealth);
                _multiCanvasController.RefreshDataPanel(actualDamage, score);
            }
        }
        else
        {
            _isAttacker = false;
            _singleCanvasController = FindObjectOfType<SingleplayerInGameCanvasManager>();
            _singleCanvasController.RefreshHealthPanel(actualHealth, maxHealth);
            _singleCanvasController.RefreshDataPanel(actualDamage, score);
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
