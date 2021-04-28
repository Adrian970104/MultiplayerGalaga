using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class IGameStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
