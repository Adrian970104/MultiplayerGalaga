using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleplayerInGameButtonController : MonoBehaviour
{
    private GameManager _gameManager;

    public void OnClickRestart()
    {
        SceneManager.LoadScene("SingleplayerInGame");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("GameMode");
    }

    #region Unty Methods
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    #endregion
}
