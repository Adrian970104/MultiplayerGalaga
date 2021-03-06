using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameMode gameMode { get; set; }
    public MultiplayerPhase multiplayerPhase { get; set; }
    public SingleplayerPhase singleplayerPhase { get; set; }

    public PhotonView photonView;


    public void StartDeploy()
    {
        //multiplayerPhase = MultiplayerPhase.InDeploy;
        photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.InDeploy);
        PhotonNetwork.LoadLevel("InGame");
    }

    public void EndMultiplayer()
    {
        photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.AfterGame);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if(gameMode == GameMode.Singleplayer)
            return;
        
        if(multiplayerPhase != MultiplayerPhase.AfterGame)
            return;
        
        var props = new[] {"IsAttacker","IsReady"};
        PhotonNetwork.RemovePlayerCustomProperties(props);
        PhotonNetwork.SetPlayerCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        SceneManager.LoadScene("GameMode");
    }

    [PunRPC]
    public void SetMultiplayerPhase(MultiplayerPhase newPhase)
    {
        multiplayerPhase = newPhase;
    }
    
    #region UnityMethods
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("GameMode");
        gameMode = GameMode.Undefined;
        multiplayerPhase = MultiplayerPhase.Undefined;
        singleplayerPhase = SingleplayerPhase.Undefined;
    }
    #endregion
}
