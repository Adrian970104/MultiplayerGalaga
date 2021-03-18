using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonConnectionHandler : MonoBehaviourPunCallbacks
{
    private static string _version = "0.2";
    public GameManager gameManager;
    public FeedbackTextController ftc;

    private void ConnectToPhoton(string connString)
    {
        Debug.Log("Connecting to Photon");
        if(PhotonNetwork.IsConnected) return;
        
        PhotonNetwork.AuthValues = new AuthenticationValues(connString);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    private void CreateRoom(string roomName)
    {

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon first!");
            return;
        }

        if (roomName.Length < 1)
        {
            Debug.Log("Need some Room name");
            return;
        }
        
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2}, TypedLobby.Default);
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LoadScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    #region Unity Methods

    private void Start()
    {
        ConnectToPhoton(_version);
        /*
        gameManager = FindObjectOfType<GameManager>();
        */
        
        //DEBUG
        gameManager = new GameManager();
        gameManager.gameMode = GameMode.Multiplayer;
        Debug.Log($"Current game mode: {gameManager.gameMode}");
    }

    private void Awake()
    {
        //Debug.Log(GameManager.gameMode);
    }

    #endregion

    #region Photon Methods

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Photon Lobby");
        CreateRoom("room1");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Room created with name {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Joined to room failed with return code: ${returnCode}");
        Debug.Log($"Joined to room failed with message: ${message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined to room: ${PhotonNetwork.CurrentRoom.Name}");
        ftc.SetFeedbackText($"Connected to room ${PhotonNetwork.CurrentRoom.Name}",Color.green);
    }

    public override void OnLeftRoom()
    {
        Debug.Log($"Room left");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: ${newPlayer.NickName}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player Left: ${otherPlayer.NickName}");
    }
    
    //OnMasterClientSwitched - Akkor történik, ha az aktuális masterClient user elhagyja a szobát. Ekkor egy másik kliens lesz a MasterClient.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"The new master client is: ${newMasterClient.NickName}");
    }
    
    #endregion
}
