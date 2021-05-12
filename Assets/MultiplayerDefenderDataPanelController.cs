using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplayerDefenderDataPanelController : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    public void Refresh(int damage)
    {
        Debug.Log($"Refreshing hp panel with damage values: {damage}");
        damageText.SetText($"Current Damage: {damage}");
    }
}
