using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class MultiplayerInGameButtonHandler : MonoBehaviour
{

    public MultiplayerInGameManager multiManager;
    public MultiplayerFeedbackPanelController _FeedbackPanelController;
    
    public GameObject spaceShip1;
    public GameObject spaceShip2;
    public GameObject spaceShip3;
    
    private AttackerBehaviour _attacker;
    private DefenderShipBehaviour _defender;
    private Vector3 _deployPos = new Vector3(-16f, 0f, 0f);
    private GameManager _gameManager;
    private MultiplayerInGameManager _multiManager;
    
    private readonly int _plusDamage = 10;
    private readonly int _plusHp = 30;
    private readonly int _scoreCost = 100;

    #region Attackers button
    private void DeploySpaceShip(GameObject ship)
    {
        if (!(_attacker.shipToDeploy is null)) 
            return;
        
        if (_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        
        if(_attacker.shipCount <= 0)
            return;

        var shipBehav = ship.GetComponent<AttackerShipBehaviour>();
        
        if(shipBehav is null)
            return;

        if (shipBehav.cost > _attacker.material)
            return;
        
        var spaceShip = PhotonNetwork.Instantiate(ship.name, _deployPos, new Quaternion(0,180,0,0));
        _attacker.shipToDeploy = spaceShip;
        --_attacker.shipCount;
        _attacker.material -= shipBehav.cost;
        _FeedbackPanelController.RefreshFeedbackPanel();
    }

    public void OnClickDeployShip1()
    {
        DeploySpaceShip(spaceShip1);
    }
    
    public void OnClickDeployShip2()
    {
        DeploySpaceShip(spaceShip2);
    }
    
    public void OnClickDeployShip3()
    {
        DeploySpaceShip(spaceShip3);
    }

    public void OnClickEndDeploy()
    {
        if(_gameManager.multiplayerPhase == MultiplayerPhase.InAttack)
            return;
        
        if(_attacker.shipToDeploy != null)
            return;
        
        if(_attacker.attackerShips.Count < 1)
            return;
        
        _gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.InAttack);
    }
    #endregion

    #region Defender buttons

    public void OnClickUpgradeHealth()
    {
        if(_defender.score < _scoreCost)
            return;
        
        _defender.photonView.RPC("AddScore", RpcTarget.All, -_scoreCost);
        _defender.photonView.RPC("IncreaseMaxHealth", RpcTarget.All, _plusHp);
    }

    public void OnClickUpgradeDamage()
    {
        if(_defender.score < _scoreCost)
            return;
        
        _defender.photonView.RPC("AddScore", RpcTarget.All, -_scoreCost);
        _defender.photonView.RPC("IncreaseDamage", RpcTarget.All, _plusDamage);
    }
    #endregion

    #region Common buttons
    public void OnClickExit()
    {
        _multiManager.photonView.RPC("EndMultiplayer",RpcTarget.Others,PhotonNetwork.PlayerListOthers[0].NickName);
        _gameManager.EndMultiplayer();
    }

    public void OnClickBackFromEnd()
    {
        _gameManager.EndMultiplayer();
    }
    #endregion

    #region Unity Methods

    public void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _multiManager = FindObjectOfType<MultiplayerInGameManager>();
        _defender = multiManager.defender;
        _attacker = multiManager.attacker;
    }

    #endregion
}