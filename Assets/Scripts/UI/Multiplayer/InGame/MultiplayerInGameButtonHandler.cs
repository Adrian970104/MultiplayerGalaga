using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MultiplayerInGameButtonHandler : MonoBehaviour
{

    public PhotonAttackerBehaviour attacker { get; set; }
    public GameObject spaceShip1;
    public GameObject spaceShip2;
    public GameObject spaceShip3;
    
    private Vector3 _deployPos = new Vector3(0f, 0f, 0f);
    private GameManager _gameManager;
    
    private void DeploySpaceShip(string shipName)
    {
        if (!(attacker.shipToDeploy is null)) 
            return;
        
        if (_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        
        var spaceShip = PhotonNetwork.Instantiate(shipName, _deployPos, Quaternion.identity);
        attacker.shipToDeploy = spaceShip;
        
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
        if(attacker.shipToDeploy != null)
            return;
        
        _gameManager.multiplayerPhase = MultiplayerPhase.InGame;
    }

    #region Unity Methods

    public void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    #endregion
}