using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerDataPanelController : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI scoreText;

    private DefenderShipBehaviour _defender;

    private readonly int _plusDamage = 10;
    private readonly int _plusHp = 30;
    private readonly int _scoreCost = 100;
    
    public void RefreshDamage(int dam)
    {
        damageText.SetText($"Current Damage: {dam}");
    }
    
    public void RefreshScore(int score)
    {
        scoreText.SetText($"Current Score: {score}");
    }

    public void OnClickUpgradeHealth()
    {
        if(_defender.score < _scoreCost)
            return;
        _defender.IncreaseMaxHealth(_plusHp);
    }

    public void OnClickUpgradeDamage()
    {
        if(_defender.score < _scoreCost)
            return;
        _defender.AddScore(-_scoreCost);
        _defender.IncreaseDamage(_plusDamage);
    }

    #region Unity Methods

    public void Start()
    {
        _defender = GameObject.FindGameObjectWithTag("DefenderShip").GetComponentInParent<DefenderShipBehaviour>();
    }

    #endregion
    
}
