using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleplayerMenuButtonHandler : MonoBehaviour
{
    private GameManager _gameManager;

    public void OnClickStart()
    {
        SceneManager.LoadScene("SingleplayerInGame");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("GameMode");
    }
    
    #region Unity Methods

    public void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    #endregion
}
