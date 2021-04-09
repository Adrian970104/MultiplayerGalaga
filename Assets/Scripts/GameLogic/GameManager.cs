using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameMode gameMode { get; set; }
    public MultiplayerPhase multiplayerPhase { get; set; }
    public SingleplayerPhase singleplayerPhase { get; set; }

    #region UnityMethods
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("GameMode");
        gameMode = GameMode.Undefined;
        multiplayerPhase = MultiplayerPhase.Undefined;
        singleplayerPhase = SingleplayerPhase.Undefined;
    }
    #endregion
}
