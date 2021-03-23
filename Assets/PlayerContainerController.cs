using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerContainerController : MonoBehaviour
{

    public GameObject UIPlayerInstance;
    
    
    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"Player Ui instance created: {player.NickName}");
            var UIplayer = Instantiate(UIPlayerInstance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
