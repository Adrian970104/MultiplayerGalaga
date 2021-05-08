using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MultiplayerInGameButtonHandler : MonoBehaviour
{

    public MultiplayerInGameManager multiManager;
    public GameObject spaceShip1;
    public GameObject spaceShip2;
    public GameObject spaceShip3;
    
    private PhotonAttackerBehaviour _attacker;
    private Vector3 _deployPos = new Vector3(0f, 0f, 0f);
    private GameManager _gameManager;

    private void DeploySpaceShip(string shipName)
    {
        if (!(_attacker.shipToDeploy is null)) 
            return;
        
        if (_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        
        if(_attacker.shipCount <= 0)
            return;
        
        var spaceShip = PhotonNetwork.Instantiate(shipName, _deployPos, Quaternion.identity);
        _attacker.shipToDeploy = spaceShip;
        --_attacker.shipCount;
        Debug.Log($"Remaining ships: {_attacker.shipCount}");
    }

    public void OnClickDeployShip1()
    {
        DeploySpaceShip(spaceShip1.name);
    }
    
    public void OnClickDeployShip2()
    {
        DeploySpaceShip(spaceShip2.name);
    }
    
    public void OnClickDeployShip3()
    {
        DeploySpaceShip(spaceShip3.name);
    }

    public void OnClickEndDeploy()
    {
        if(_gameManager.multiplayerPhase == MultiplayerPhase.InGame)
            return;
        
        if(_attacker.shipToDeploy != null)
            return;
        
        _gameManager.multiplayerPhase = MultiplayerPhase.InGame;
    }

    public void OnClickBackFromEnd()
    {
        _gameManager.EndMultiplayer();
    }

    #region Unity Methods

    public void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if(!multiManager.isAttacker)
            return;
        _attacker = multiManager.attacker;
    }

    #endregion
}