using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameMode gameMode { get; set; }
    public MultiplayerPhase multiplayerPhase { get; set; }
    public SingleplayerPhase singleplayerPhase { get; set; }
    
    
    public void StartDeploy()
    {
        multiplayerPhase = MultiplayerPhase.InDeploy;
        PhotonNetwork.LoadLevel("InGame");
    }

    public void EndMultiplayer()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("GameMode");
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

    private void Update()
    {
    }

    #endregion
}
