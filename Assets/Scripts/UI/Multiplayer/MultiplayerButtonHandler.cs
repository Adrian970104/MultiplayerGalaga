using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerButtonHandler : MonoBehaviour
{
    public TextMeshProUGUI usernameField;
    public TextMeshProUGUI roomnamField;
    public PhotonConnectionHandler photonConnHandler;
    
    public void OnClickJoinRoom()
    {
        if(!PhotonNetwork.IsConnected) return;

        photonConnHandler.JoinRoom(roomnamField.text, usernameField.text);
    }
    
    public void OnClickCreateRoom()
    {
        if(!PhotonNetwork.IsConnected) return;

        photonConnHandler.CreateRoom(roomnamField.text, usernameField.text);
    }
    
    public void OnClickBackFromMultiplayer()
    {
        if(PhotonNetwork.InRoom) return;
        
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("GameMode");
    }
    
    public void OnClickBackFromLobby()
    {
        photonConnHandler.LeaveRoom();
    }
    
    private void ClearButtonTexts()
    {
        usernameField.text = string.Empty;
        roomnamField.text = string.Empty;
    }
    
    #region Unity Methods

    #endregion
}
