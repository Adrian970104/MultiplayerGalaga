using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CurrentRoomController : MonoBehaviour
{
    public TextMeshProUGUI CurrentRoomText;

    #region Unity Methods
    private void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {
            CurrentRoomText.text = "";
            return;
        }

        if (PhotonNetwork.CurrentRoom is null)
        {
            CurrentRoomText.text = "";
            return;
        }
        
        CurrentRoomText.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";
        CurrentRoomText.color = Color.green;
    }
    #endregion
}
