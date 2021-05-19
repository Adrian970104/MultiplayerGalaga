using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MultiplayerInGameButtonHandler : MonoBehaviour
{

    public MultiplayerInGameManager multiManager;
    public MultiplayerFeedbackPanelController _FeedbackPanelController;
    
    //TODO Adrián ezt listával megoldani !
    public GameObject spaceShip1;
    public GameObject spaceShip2;
    public GameObject spaceShip3;
    
    private PhotonAttackerBehaviour _attacker;
    private DefenderShipBehaviour _defender;
    private Vector3 _deployPos = new Vector3(-16f, 0f, 0f);
    private GameManager _gameManager;
    
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
        if(_gameManager.multiplayerPhase == MultiplayerPhase.InGame)
            return;
        
        if(_attacker.shipToDeploy != null)
            return;
        
        if(_attacker.attackerShips.Count < 1)
            return;
        
        _gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.InGame);
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
        /*if(!multiManager.isAttacker)
            return;*/
        _defender = multiManager.defender;
        _attacker = multiManager.attacker;
    }

    #endregion
}