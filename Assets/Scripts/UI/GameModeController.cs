using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeController : MonoBehaviour
{
    public GameManager gameManager;

    public void OnClickSingelplayer()
    {
        gameManager.gameMode = GameMode.Singleplayer;
        SceneManager.LoadScene("SingleplayerMenu");
    }

    public void OnClickMultiplayer()
    {
        gameManager.gameMode = GameMode.Multiplayer;
        SceneManager.LoadScene("MultiplayerMenu");
    }

    #region UntiyMethods
    
    #endregion
}
