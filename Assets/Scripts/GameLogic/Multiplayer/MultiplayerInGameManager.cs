using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerInGameManager : MonoBehaviour
{
    public PhotonView photonView;
    public MultiplayerInGameCanvasManager CanvasManager;
    
    public GameObject defenderPrefab;
    public GameObject attackerPrefab;

    public bool isAttacker;
    public PhotonAttackerBehaviour attacker;
    public DefenderShipBehaviour defender;

    private GameManager _gameManager;
    private Vector3 _attackerInstPos = new Vector3(20f,20f,20f);
    private readonly Vector3 _defenderInstPos = new Vector3(0f,0f,-15f);

    [PunRPC]
    public void EndMultiplayer(string winner)
    {
        _gameManager.multiplayerPhase = MultiplayerPhase.AfterGame;
        CanvasManager.FillWinnerCanvas(winner);
        CanvasManager.SetWinnerCanvasVisible();
    }
    
    #region Untiy Methods
    void Start()
    {
        Debug.Log($"InGAME Manager START running;");
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.multiplayerPhase = MultiplayerPhase.InDeploy;
        
        isAttacker  = (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"];

        if (isAttacker)
        {
            CanvasManager.SetAttackerCanvasVisible();
            attacker = PhotonNetwork.Instantiate(attackerPrefab.name, _attackerInstPos, Quaternion.identity).GetComponent<PhotonAttackerBehaviour>();
        }
        else
        {
            CanvasManager.SetDefenderCanvasVisible();
            defender = PhotonNetwork.Instantiate(defenderPrefab.name, _defenderInstPos, Quaternion.identity).GetComponent<DefenderShipBehaviour>();
        }
    }
    #endregion
}
