﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerDataPanelController : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI scoreText;
    
    public void RefreshDamage(int dam)
    {
        damageText.SetText($"Current Damage: {dam}");
    }
    
    public void RefreshScore(int score)
    {
        scoreText.SetText($"Current Score: {score}");
    }

}
