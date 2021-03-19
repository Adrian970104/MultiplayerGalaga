using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public void OnClickBack()
    {
        if(PhotonNetwork.InRoom) return;
        
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("GameMode");
    }
    
    #region Unity Methods

    #endregion
}
