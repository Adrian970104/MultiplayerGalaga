using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CurrentRoomTextController : MonoBehaviour
{
    public TextMeshProUGUI CurrentRoomText;
    public void Refresh()
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
        
        CurrentRoomText.text = $"Current Room: {PhotonNetwork.CurrentRoom.Name}";
        CurrentRoomText.color = Color.green;
    }
}
