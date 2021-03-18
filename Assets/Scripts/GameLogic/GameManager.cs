using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode gameMode { get; set; }

    #region UnityMethods
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
}
