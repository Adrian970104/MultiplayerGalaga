using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string name { get; set; }
    private Player photonPlayer { get; set; }
    private PlayerRole role = PlayerRole.Undefined;
    
    #region UnityMethods
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion
}
