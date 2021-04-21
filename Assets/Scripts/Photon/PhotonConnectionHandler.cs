using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonConnectionHandler : MonoBehaviourPunCallbacks
{
    private static string _version = "0.2";
    public GameManager gameManager;
    public FeedbackTextController ftc;
    public Canvas serverCreationCanvas;
    public Canvas lobbyCanvas;
    public PlayerContainerController playerContainerController;
    
    private readonly RoomOptions _roomOptions = new RoomOptions(){MaxPlayers = 2};

    private void ConnectToPhoton(string connString)
    {
        Debug.Log("Connecting to Photon");
        if(PhotonNetwork.IsConnected) return;
        
        PhotonNetwork.AuthValues = new AuthenticationValues(connString);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    private bool CheckUserInput(string input)
    {
        var regex = new Regex("[a-zA-Z0-9]+");
        
        return regex.IsMatch(input);
    }
    
    public void CreateRoom(string roomName, string nickname)
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon first!");
            ftc.SetPhotonError("Connect to Photon first!", Color.red);
            return;
        }

        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Connect to Photon first!");
            ftc.SetPhotonError("Connect to Photon first!", Color.red);
            return;
        }

        if (!CheckUserInput(roomName))
        {
            Debug.Log("Invalid Room Name!");
            ftc.SetPhotonError("Invalid Room Name!", Color.red);
            return;
        }
        
        if (!CheckUserInput(nickname))
        {
            Debug.Log("Invalid Username!");
            ftc.SetPhotonError("Invalid Username!", Color.red);
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
            ftc.SetPhotonError("Connect to Photon first!", Color.red);
            return;
        }
        
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Connect to Photon first!");
            ftc.SetPhotonError("Connect to Photon first!", Color.red);
            return;
        }

        if (!CheckUserInput(roomName))
        {
            Debug.Log("Invalid Room Name!");
            ftc.SetPhotonError("Invalid Room Name!", Color.red);
            return;
        }
        
        if (!CheckUserInput(nickname))
        {
            Debug.Log("Invalid Username!");
            ftc.SetPhotonError("Invalid Username!", Color.red);
            return;
        }
        
        PhotonNetwork.JoinRoom(roomName.ToLower());
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon first!");
            ftc.SetPhotonError("Connect to Photon first!", Color.red);
            return;
        }
        
        if(!PhotonNetwork.IsConnectedAndReady) return;

        PhotonNetwork.LocalPlayer.NickName = string.Empty;
        PhotonNetwork.LeaveRoom();
    }

    public void LoadScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    private void ChangeToLobbyCanvas()
    {
        lobbyCanvas.enabled = true;
        serverCreationCanvas.enabled = false;
    }

    private void ChangeToServerCreationCanvas()
    {
        serverCreationCanvas.enabled = true;
        lobbyCanvas.enabled = false;
    }

    private bool PlayerCountCheck()
    {
        Debug.Log($"PlayerListLength: {PhotonNetwork.PlayerList.Length}");
        Debug.Log($"MaxPlayers: {_roomOptions.MaxPlayers}");
        return PhotonNetwork.PlayerList.Length == _roomOptions.MaxPlayers;
    }

    private bool PlayerReadyCheck()
    {
        if (PhotonNetwork.PlayerList[0].CustomProperties["IsReady"] is null) return false;
        if (PhotonNetwork.PlayerList[1].CustomProperties["IsReady"] is null) return false;
        return (bool) PhotonNetwork.PlayerList[0].CustomProperties["IsReady"] &&
               (bool) PhotonNetwork.PlayerList[1].CustomProperties["IsReady"];
    }

    private bool PlayerRoleCheck()
    {
        if (PhotonNetwork.PlayerList[0].CustomProperties["IsAttacker"] is null) return false;
        if (PhotonNetwork.PlayerList[1].CustomProperties["IsAttacker"] is null) return false;
        return (bool) PhotonNetwork.PlayerList[0].CustomProperties["IsAttacker"] !=
               (bool) PhotonNetwork.PlayerList[1].CustomProperties["IsAttacker"];
    }

    #region Unity Methods

    private void Start()
    {
        ConnectToPhoton(Guid.NewGuid().ToString());
        PhotonNetwork.GameVersion = _version;
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
        ChangeToServerCreationCanvas();
        playerContainerController.Clear();
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
        ftc.SetPhotonStatus("Connected To Photon Master Server",Color.green);
        if (PhotonNetwork.InLobby)
        {
            ftc.SetPhotonStatus("Connected To Photon Lobby",Color.green);
            return;
        }
        ftc.SetPhotonStatus("Connecting To Photon Lobby...",Color.yellow);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Photon Lobby");
        ftc.SetPhotonStatus("Connected To Photon Lobby",Color.green);
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
        
        if (message.Contains("A game with the specified id already exist"))
        {
            PhotonNetwork.JoinLobby();
            ftc.SetPhotonError("Roomname reserved",Color.red);
            return;
        }
        
        PhotonNetwork.JoinLobby();
        ftc.SetPhotonError("Room creation failed",Color.red);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Joined to room failed with return code: ${returnCode}");
        Debug.Log($"Joined to room failed with message: ${message}");

        if (message.Contains("Game does not exist"))
        {
            PhotonNetwork.JoinLobby();
            ftc.SetPhotonError("Room does not exist",Color.red);
            return;
        }
        
        PhotonNetwork.JoinLobby();
        ftc.SetPhotonError("Failed to join room",Color.red);
    }

    public override void OnJoinedRoom()
    {
        
        if (PhotonNetwork.PlayerListOthers.Any(player => player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName)))
        {
            LeaveRoom();
            Debug.Log("Username reserved!");
            ftc.SetPhotonError("Username reserved!", Color.red);
            return;
        }
        
        Debug.Log($"Joined to room: ${PhotonNetwork.CurrentRoom.Name}");
        ftc.SetPhotonStatus($"Connected to room ${PhotonNetwork.CurrentRoom.Name}",Color.green);
        ftc.ClearPhotonError();
        ftc.ClearPhotonStatus();
        ChangeToLobbyCanvas();
        playerContainerController.Fill();
    }

    public override void OnLeftRoom()
    {
        var props = new[] {"IsAttacker","IsReady"};
        Debug.Log($"Room left");
        Debug.Log($"CustomProperties reseted");
        PhotonNetwork.RemovePlayerCustomProperties(props);
        ChangeToServerCreationCanvas();
        playerContainerController.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: ${newPlayer.NickName}");
        //playerContainerController.Refresh();
        playerContainerController.photonView.RPC("Refresh",RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player Left: ${otherPlayer.NickName}");
        //playerContainerController.Refresh();
        playerContainerController.photonView.RPC("Refresh",RpcTarget.All);
    }
    
    //OnMasterClientSwitched - Akkor történik, ha az aktuális masterClient user elhagyja a szobát. Ekkor egy másik kliens lesz a MasterClient.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"The new master client is: ${newMasterClient.NickName}");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!changedProps.ContainsKey("IsReady")) return;
        if (!PlayerCountCheck())
        {
            Debug.Log($"Kevés user.");
            return;
        }

        if (!PlayerReadyCheck())
        {
            Debug.Log($"Várakozás a másik játékosra");
            return;
        }

        if (!PlayerRoleCheck())
        {
            Debug.Log($"Nem lehet ugyanaz a role-ja két játékosnak.");
            return;
        }
        gameManager.StartDeploy();
    }

    #endregion
}
