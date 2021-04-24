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
            Debug.Log($"Player {player.NickName} is Attacker: {(bool)player.CustomProperties["IsAttacker"]}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
