using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeButtonHandler : MonoBehaviour
{
    public GameManager gameManager;

    public void OnClickSingelplayer()
    {
        gameManager.gameMode = GameMode.Singleplayer;
        gameManager.multiplayerPhase = MultiplayerPhase.Undefined;
        SceneManager.LoadScene("SingleplayerMenu");
    }

    public void OnClickMultiplayer()
    {
        gameManager.gameMode = GameMode.Multiplayer;
        gameManager.singleplayerPhase = SingleplayerPhase.Undefined;
        SceneManager.LoadScene("MultiplayerMenu");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    #region UntiyMethods

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.gameMode = GameMode.Undefined;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    #endregion
}
