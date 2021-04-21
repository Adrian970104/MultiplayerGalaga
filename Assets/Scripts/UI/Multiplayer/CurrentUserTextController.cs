using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CurrentUserTextController : MonoBehaviour
{
    public TextMeshProUGUI CurrentUserText;

    public void Refresh()
    {
        if (!PhotonNetwork.IsConnected)
        {
            CurrentUserText.text = "";
            return;
        }

        if (PhotonNetwork.CurrentRoom is null)
        {
            CurrentUserText.text = "";
            return;
        }
        
        CurrentUserText.text = $"Current User: {PhotonNetwork.LocalPlayer.NickName}";
        CurrentUserText.color = Color.green;
    }
}
