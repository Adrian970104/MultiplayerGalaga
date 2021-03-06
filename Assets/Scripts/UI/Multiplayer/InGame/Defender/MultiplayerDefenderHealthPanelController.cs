using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerDefenderHealthPanelController : MonoBehaviour
{

    public Slider healthBar;
    public TextMeshProUGUI healthText;

    public void Refresh(int actualHealth, int maxHealth)
    {
        Debug.Log($"Refreshing hp panel with values: {actualHealth}/{maxHealth}");
        healthBar.value = (float)actualHealth / maxHealth;
        healthText.SetText($"Health: {actualHealth}/{maxHealth}");
    }
}
