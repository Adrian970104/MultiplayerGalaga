using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class RadyCheckFeedbackTextController : MonoBehaviour
{
    public TextMeshProUGUI readyCheckFeedbackText;
    
    public void SetMessage(string message)
    {
        readyCheckFeedbackText.text = message;
        readyCheckFeedbackText.color = Color.yellow;
    }
}
