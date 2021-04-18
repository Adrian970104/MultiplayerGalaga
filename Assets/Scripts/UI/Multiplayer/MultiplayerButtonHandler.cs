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
    public Button readyButton;
    
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
    
    public void ClearButtonTexts()
    {
        usernameField.text = string.Empty;
        roomnamField.text = string.Empty;
    }

    public void OnClickReady()
    {
        //ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();
        
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("IsReady"))
        {
            ChangeReady(false);
        }
        else
        {
            ChangeReady((bool) PhotonNetwork.LocalPlayer.CustomProperties["IsReady"]);
        }
    }

    private void ChangeReady(bool isReady)
    { 
        ExitGames.Client.Photon.Hashtable myCustomProperties = new ExitGames.Client.Photon.Hashtable();
        //UIPlayerInstanceController uiplController = new UIPlayerInstanceController();
        
        myCustomProperties["IsReady"] = !isReady;
        PhotonNetwork.SetPlayerCustomProperties(myCustomProperties);
        //PhotonNetwork.LocalPlayer.CustomProperties = myCustomProperties;

        foreach (var plinstance in GameObject.FindGameObjectsWithTag("UIPlayerInstance"))
        {
            var uiplic = plinstance.GetComponent<UIPlayerInstanceController>();
            if (uiplic.username.text == PhotonNetwork.LocalPlayer.NickName)
            {
                uiplic.SetCbSelectable(isReady);
            }
        }
        
        if (isReady)
        {
            readyButton.image.color = Color.green;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }
        else
        {
            readyButton.image.color = Color.yellow;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel Ready";
        }
    }

    #region Unity Methods

    #endregion
}
