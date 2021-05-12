using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleplayerHealthPanelController : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    public void Refresh(int actualHealth, int maxHealth)
    {
        healthBar.value = (float)actualHealth / maxHealth;
        healthText.SetText($"Health: {actualHealth}/{maxHealth}");
    }
}
