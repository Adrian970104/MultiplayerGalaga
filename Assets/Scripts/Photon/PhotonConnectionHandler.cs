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
    public Canvas sercerCreationCanvas;
    public Canvas lobbyCanvas;
    
    private readonly RoomOptions _roomOptions = new RoomOptions(){MaxPlayers = 2};

    private void ConnectToPhoton(string connString)
    {
        Debug.Log("Connecting to Photon");
        if(PhotonNetwork.IsConnected) return;
        
        PhotonNetwork.AuthValues = new AuthenticationValues(connString);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public void CreateRoom(string roomName, string nickname)
    {

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon first!");
            ftc.SetFeedbackText("Connect to Photon first!", Color.red);
            return;
        }

        if (roomName.Length < 2)
        {
            Debug.Log("Room name can not be empty!");
            ftc.SetFeedbackText("Room name can not be empty", Color.red);
            return;
        }
        
        if (nickname.Length < 2)
        {
            Debug.Log("Username can not be empty!");
            ftc.SetFeedbackText("Username can not be empty!", Color.red);
            return;
        }
        
        PhotonNetwork.CreateRoom(roomName.ToLower(), _roomOptions, TypedLobby.Default);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void JoinRoom(string roomName, string nickname)
    {
        if(PhotonNetwork.InRoom) return;
        
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon first!");
            ftc.SetFeedbackText("Connect to Photon first!", Color.red);
            return;
        }

        if (roomName.Length < 2)
        {
            Debug.Log("Room name can not be empty!");
            ftc.SetFeedbackText("Room name can not be empty", Color.red);
            return;
        }
        
        if (nickname.Length < 2)
        {
            Debug.Log("Username can not be empty!");
            ftc.SetFeedbackText("Username can not be empty!", Color.red);
            return;
        }
        
        PhotonNetwork.JoinRoom(roomName.ToLower());
        PhotonNetwork.LocalPlayer.NickName = nickname;
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
        */
        gameManager = FindObjectOfType<GameManager>();
        
        /*
        //DEBUG
        gameManager = new GameManager();
        */
        //DEBUG
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
        ftc.SetFeedbackText("Connected To Photon Master Server",Color.green);
        if (PhotonNetwork.InLobby)
        {
            ftc.SetFeedbackText("Connected To Photon Lobby",Color.green);
            return;
        }
        ftc.SetFeedbackText("Connecting To Photon Lobby...",Color.yellow);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Photon Lobby");
        ftc.SetFeedbackText("Connected To Photon Lobby",Color.green);
        //CreateRoom("room1");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Room created with name {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Room creation failed with return code: ${returnCode}");
        Debug.Log($"Room creation failed with message: ${message}");

        ftc.SetFeedbackText("Room creation failed",Color.red);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Joined to room failed with return code: ${returnCode}");
        Debug.Log($"Joined to room failed with message: ${message}");

        if (message.Contains("Game does not exist"))
        {
            ftc.SetFeedbackText("Room does not exist",Color.red);
            return;
        }
        
        ftc.SetFeedbackText("Failed to join room",Color.red);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined to room: ${PhotonNetwork.CurrentRoom.Name}");
        ftc.SetFeedbackText($"Connected to room ${PhotonNetwork.CurrentRoom.Name}",Color.green);
        sercerCreationCanvas.enabled = false;
        lobbyCanvas.enabled = true;
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
