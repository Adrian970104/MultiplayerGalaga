using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackTextController : MonoBehaviour
{
    public TextMeshProUGUI photonStatus;
    public TextMeshProUGUI photonError;
    
    public void SetPhotonStatus(string text, Color color)
    {
        photonStatus.text = text;
        photonStatus.color = color;
    }
    
    public void SetPhotonError(string text, Color color)
    {
        photonError.text = text;
        photonError.color = color;
    }

    public void ClearPhotonError()
    {
        photonError.text = string.Empty;
    }
    public void ClearPhotonStatus()
    {
        photonStatus.text = string.Empty;
    }
}
