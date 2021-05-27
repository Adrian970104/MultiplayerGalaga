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
    public CurrentRoomTextController crtc;
    public CurrentUserTextController cutc;
    public RadyCheckFeedbackTextController rcftc;
    
    private readonly RoomOptions _roomOptions = new RoomOptions(){MaxPlayers = 2};

    private void ConnectToPhoton(string connString)
    {
        Debug.Log("Connecting to Photon");
        if(PhotonNetwork.IsConnected) 
            return;

        PhotonNetwork.AuthValues = new AuthenticationValues(connString);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    private bool HaveInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    private bool CheckUserInput(string input)
    {
        var regex = new Regex("^[a-zA-Z0-9]+$");
        
        return regex.IsMatch(input);
    }
    
    public void CreateRoom(string roomName, string nickname)
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon First");
            ftc.SetPhotonError("Connect to Photon First", Color.red);
            return;
        }

        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Connect to Photon first");
            ftc.SetPhotonError("Connect to Photon First", Color.red);
            return;
        }

        if (!CheckUserInput(roomName))
        {
            Debug.Log("Invalid Room Name");
            ftc.SetPhotonError("Invalid Room Name", Color.red);
            return;
        }
        
        if (!CheckUserInput(nickname))
        {
            Debug.Log("Invalid Username");
            ftc.SetPhotonError("Invalid Username", Color.red);
            return;
        }
        
        PhotonNetwork.CreateRoom(roomName.ToLower(), _roomOptions, TypedLobby.Default);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void JoinRoom(string roomName, string nickname)
    {
        
        if(PhotonNetwork.InRoom)
            return;
        
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon first");
            ftc.SetPhotonError("Connect to Photon first", Color.red);
            return;
        }
        
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Connect to Photon first");
            ftc.SetPhotonError("Connect to Photon first", Color.red);
            return;
        }

        if (!CheckUserInput(roomName))
        {
            Debug.Log("Invalid Room Name");
            ftc.SetPhotonError("Invalid Room Name", Color.red);
            return;
        }
        
        if (!CheckUserInput(nickname))
        {
            Debug.Log("Invalid Username");
            ftc.SetPhotonError("Invalid Username", Color.red);
            return;
        }
        
        PhotonNetwork.JoinRoom(roomName.ToLower());
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connect to Photon first");
            ftc.SetPhotonError("Connect to Photon first", Color.red);
            return;
        }
        
        if(!PhotonNetwork.IsConnectedAndReady)
            return;
        
        Debug.Log("Leaving room");

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
        return PhotonNetwork.PlayerList.Length == _roomOptions.MaxPlayers;
    }

    private bool PlayerReadyCheck()
    {
        if (PhotonNetwork.PlayerList.Any(player => player.CustomProperties["IsReady"] is null))
        {
            return false;
        }

        return PhotonNetwork.PlayerList.All(player => (bool) player.CustomProperties["IsReady"]);
    }

    private bool PlayerRoleCheck()
    {
        if (PhotonNetwork.PlayerList.Any(player => player.CustomProperties["IsAttacker"] is null))
        {
            return false;
        }
        return (bool) PhotonNetwork.PlayerList[0].CustomProperties["IsAttacker"] !=
               (bool) PhotonNetwork.PlayerList[1].CustomProperties["IsAttacker"];
    }

    #region Unity Methods

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.gameMode = GameMode.Multiplayer;
        gameManager.multiplayerPhase = MultiplayerPhase.InRoomCreation;
        
        ChangeToServerCreationCanvas();
        playerContainerController.Clear();
        
        if (!HaveInternet())
        {
            Debug.Log("No internet connection");
            ftc.SetPhotonStatus("No Internet Connection",Color.red);
            return;
        }
        
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log($"Already connected to photon");
            PhotonNetwork.Disconnect();
        }
        
        ConnectToPhoton(Guid.NewGuid().ToString());
        PhotonNetwork.GameVersion = _version;
    }
    #endregion

    #region Photon Methods

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        ftc.SetPhotonStatus("Connected To Photon Master Server",Color.green);
        if (PhotonNetwork.InLobby)
        {
            ftc.SetPhotonStatus("Connected To Photon",Color.green);
            return;
        }
        ftc.SetPhotonStatus("Connecting To Photon...",Color.yellow);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Photon Lobby");
        ftc.SetPhotonStatus("Connected To Photon",Color.green);
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
            ftc.SetPhotonError("Room Name Reserved", Color.red);
            return;
        }
        
        PhotonNetwork.JoinLobby();
        ftc.SetPhotonError("Room Creation Failed", Color.red);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Joined to room failed with return code: ${returnCode}");
        Debug.Log($"Joined to room failed with message: ${message}");

        if (message.Contains("Game does not exist"))
        {
            PhotonNetwork.JoinLobby();
            ftc.SetPhotonError("Room Does Not Exist", Color.red);
            return;
        }
        
        if (message.Contains("full"))
        {
            PhotonNetwork.JoinLobby();
            ftc.SetPhotonError("Room Is Full", Color.red);
            return;
        }
        
        if (message.Contains("Game closed"))
        {
            PhotonNetwork.JoinLobby();
            ftc.SetPhotonError("Game Is Not Ended Yet", Color.red);
            return;
        }
        
        PhotonNetwork.JoinLobby();
        ftc.SetPhotonError("Failed To Join Room", Color.red);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.PlayerListOthers.Any(player => player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName)))
        {
            LeaveRoom();
            Debug.Log("Username reserved");
            ftc.SetPhotonError("Username Reserved", Color.red);
            return;
        }
        
        Debug.Log($"Joined to room: ${PhotonNetwork.CurrentRoom.Name}");
        ftc.SetPhotonStatus($"Connected to room ${PhotonNetwork.CurrentRoom.Name}",Color.green);
        ftc.ClearPhotonError();
        ftc.ClearPhotonStatus();
        cutc.Refresh();
        crtc.Refresh();
        rcftc.SetMessage(string.Empty);
        
        ChangeToLobbyCanvas();
        playerContainerController.Fill();
        //TODO Adrián Ezt megnézni, hogy PUnRPC nélkül múködik-e? Jó lenne ha igen!
        gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.InRoom);
    }

    public override void OnLeftRoom()
    {
        Debug.Log($"Room left");
        
        var props = new[] {"IsAttacker","IsReady"};
        PhotonNetwork.RemovePlayerCustomProperties(props);
        PhotonNetwork.SetPlayerCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        ChangeToServerCreationCanvas();
        playerContainerController.Clear();
        //TODO Adrián Ezt megnézni, hogy PUnRPC nélkül múködik-e? Jó lenne ha igen!
        gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.InRoomCreation);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: ${newPlayer.NickName}");
        playerContainerController.photonView.RPC("Refresh",RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player Left: ${otherPlayer.NickName}");
        playerContainerController.photonView.RPC("Refresh",RpcTarget.All);
    }
    
    //OnMasterClientSwitched - Akkor történik, ha az aktuális masterClient user elhagyja a szobát. Ekkor egy másik kliens lesz a MasterClient.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"The new master client is: ${newMasterClient.NickName}");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!changedProps.ContainsKey("IsReady"))
            return;

        if (!PlayerCountCheck())
        {
            rcftc.SetMessage($"Two players required");
            return;
        }

        if (!PlayerReadyCheck())
        {
            if(!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("IsReady"))
                return;
            
            if ((bool) PhotonNetwork.LocalPlayer.CustomProperties["IsReady"])
            {
                rcftc.SetMessage("Waiting for other player");
            }
            else
            {
                rcftc.SetMessage(string.Empty);
            }

            return;
        }
        
        if (!PlayerRoleCheck())
        {
            rcftc.SetMessage("Players must choose different roles");
            return;
        }
        
        gameManager.StartDeploy();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from photon with cause: {cause}");
        ftc.SetPhotonStatus("Disconnected From Photon", Color.red);
    }

    public override void OnConnected()
    {
        Debug.Log($"On connected happend");
    }
    #endregion
}
