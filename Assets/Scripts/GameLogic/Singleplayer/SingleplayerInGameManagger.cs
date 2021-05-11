using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SingleplayerInGameManagger : MonoBehaviour
{
    public GameObject defenderShip;
    
    public GameObject attackerShip1;
    public GameObject attackerShip2;
    public GameObject attackerShip3;

    private List<SpaceShip> attackerShips = new List<SpaceShip>();
    
    private GameManager _gameManager;
    private Vector3 _defenderPos = new Vector3(0,0,-15);

    private void attackerSetup()
    {
        var att1 = PhotonNetwork.Instantiate(attackerShip1.name, new Vector3(-10, 0, 0), new Quaternion(0,180,0,0));
        var att2 = PhotonNetwork.Instantiate(attackerShip2.name, new Vector3(0, 0, 0), new Quaternion(0,180,0,0));
        var att3 = PhotonNetwork.Instantiate(attackerShip3.name, new Vector3(10, 0, 0), new Quaternion(0,180,0,0));
        
        attackerShips.Add(att1.GetComponent<SpaceShip>());
        attackerShips.Add(att2.GetComponent<SpaceShip>());
        attackerShips.Add(att3.GetComponent<SpaceShip>());
    }

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.JoinRandomRoom();
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.singleplayerPhase = SingleplayerPhase.InGame;
        PhotonNetwork.Instantiate(defenderShip.name, _defenderPos, Quaternion.identity);
        attackerSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
