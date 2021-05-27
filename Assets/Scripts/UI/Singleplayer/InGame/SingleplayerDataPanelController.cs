using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerDataPanelController : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI scoreText;

    public void Refresh(int dam, int score)
    {
        damageText.SetText($"Damage: {dam}");
        scoreText.SetText($"Score: {score}");
    }
}
