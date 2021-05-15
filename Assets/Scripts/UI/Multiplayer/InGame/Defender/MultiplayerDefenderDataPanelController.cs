using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplayerDefenderDataPanelController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI damageText;

    public void Refresh(int damage, int score)
    {
        Debug.Log($"Refreshing hp panel with damage values: {damage}, {score}");
        damageText.SetText($"Damage: {damage}");
        scoreText.SetText($"Score: {score}");
    }
}
