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
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //Debug.Log($"Current multiplayer phase is : {multiplayerPhase}");
        if (multiplayerPhase != MultiplayerPhase.AfterGame)
        {
            Debug.Log($"Multiplayer phase is not After Game dont need the first scene : {multiplayerPhase}");
            return;
        }
        Debug.Log($"Multiplayer phase is After Game we need the first scene : {multiplayerPhase}");
        var props = new[] {"IsAttacker","IsReady"};
        PhotonNetwork.RemovePlayerCustomProperties(props);
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.ToString());
        PhotonNetwork.SetPlayerCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        Debug.Log($"CustomProperties reseted");
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
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("GameMode");
        gameMode = GameMode.Undefined;
        multiplayerPhase = MultiplayerPhase.Undefined;
        singleplayerPhase = SingleplayerPhase.Undefined;
    }
    #endregion
}
