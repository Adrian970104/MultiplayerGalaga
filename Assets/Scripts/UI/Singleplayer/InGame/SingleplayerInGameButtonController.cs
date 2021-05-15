using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleplayerInGameButtonController : MonoBehaviour
{
    private GameManager _gameManager;
    private DefenderShipBehaviour _defender;
    
    private readonly int _plusDamage = 10;
    private readonly int _plusHp = 30;
    private readonly int _scoreCost = 100;

    public void OnClickRestart()
    {
        SceneManager.LoadScene("SingleplayerInGame");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("GameMode");
    }
    
    public void OnClickUpgradeHealth()
    {
        if(_defender.score < _scoreCost)
            return;
        
        _defender.AddScore(-_scoreCost);
        _defender.IncreaseMaxHealth(_plusHp);
    }

    public void OnClickUpgradeDamage()
    {
        if(_defender.score < _scoreCost)
            return;
        
        _defender.AddScore(-_scoreCost);
        _defender.IncreaseDamage(_plusDamage);
    }

    public void OnClickExit()
    {
        _gameManager.singleplayerPhase = SingleplayerPhase.AfterGame;
        SceneManager.LoadScene("GameMode");
    }
    

    #region Unty Methods
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _defender = GameObject.FindGameObjectWithTag("DefenderShip").GetComponentInParent<DefenderShipBehaviour>();
    }
    #endregion
}
