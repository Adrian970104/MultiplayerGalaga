using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerDataPanelController : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    
    public void Refresh(int dam)
    {
        damageText.SetText($"Current Damage: {dam}");
    }
}
