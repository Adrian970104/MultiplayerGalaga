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
    public TMP_InputField usernameField;
    public TMP_InputField roomnameField;
    public PhotonConnectionHandler photonConnHandler;
    public Button readyButton;
    
    public void OnClickJoinRoom()
    {
        if(!PhotonNetwork.IsConnected)
            return;
        photonConnHandler.JoinRoom(roomnameField.text, usernameField.text);
        ActivateReadyButton(false);
    }
    
    public void OnClickCreateRoom()
    {
        if(!PhotonNetwork.IsConnected)
            return;
        photonConnHandler.CreateRoom(roomnameField.text, usernameField.text);
        ActivateReadyButton(false);
    }
    
    public void OnClickBackFromMultiplayer()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"Player still in room. {PhotonNetwork.CurrentRoom.Name}");
            return;
        }

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("GameMode");
    }
    
    public void OnClickBackFromLobby()
    {
        SetReadyButton(true);
        photonConnHandler.LeaveRoom();
    }
    
    public void ClearButtonTexts()
    {
        usernameField.text = string.Empty;
        roomnameField.text = string.Empty;
    }

    public void OnClickReady()
    {
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

        myCustomProperties["IsReady"] = !isReady;
        PhotonNetwork.SetPlayerCustomProperties(myCustomProperties);

        foreach (var plinstance in GameObject.FindGameObjectsWithTag("UIPlayerInstance"))
        {
            var uiplic = plinstance.GetComponent<UIPlayerInstanceController>();
            if (uiplic.username.text == PhotonNetwork.LocalPlayer.NickName)
            {
                uiplic.SetCbSelectable(isReady);
            }
        }
        
        SetReadyButton(isReady);
    }

    private void SetReadyButton(bool isReady)
    {
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

    private void ActivateReadyButton(bool activate)
    {
        readyButton.interactable = activate;
    }

    #region Unity Methods

    #endregion
}
