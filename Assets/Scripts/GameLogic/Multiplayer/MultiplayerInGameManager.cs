using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerInGameManager : MonoBehaviourPunCallbacks
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
        _gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.AfterGame);
        CanvasManager.FillWinnerCanvas(winner);
        CanvasManager.SetWinnerCanvasVisible();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InGame)
            return;
        EndMultiplayer(PhotonNetwork.PlayerList[0].NickName);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"New Master client is {newMasterClient.NickName}");
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
        
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    #endregion
}
