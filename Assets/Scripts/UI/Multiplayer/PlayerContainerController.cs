using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerContainerController : MonoBehaviour
{
    [SerializeField]
    public GameObject UIPlayerInstance; 
    
    [SerializeField]
    public GameObject localUIPlayerInstance;
    [SerializeField]
    public UIPlayerInstanceController localUIPlayerInstanceController;

    [SerializeField] public bool isAttacker;

    public string alma = "alma";
    
    private ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();

    public PhotonView photonView;
    public GameObject content;

    public void Clear()
    {
        var UIPlayers = GameObject.FindGameObjectsWithTag("UIPlayerInstance");
        foreach (var UIPlayer in UIPlayers)
        {
            Debug.Log($"Player Ui instance destroyed: {UIPlayer.name}");
            Destroy(UIPlayer);
        }
    }
    
    public void Fill()
    {
        foreach (var plr in PhotonNetwork.PlayerList)
        {
            Debug.Log($"Player in playerList: {plr.NickName}");
        }
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            /*if(player.NickName == PhotonNetwork.LocalPlayer.NickName && !player.Equals(PhotonNetwork.LocalPlayer))
                return;*/
            
            Debug.Log($"Player Ui instance created: {player.NickName}");
            //var canvasTransform = canvas.transform;
            var contentTransform = content.transform;
            //var UIPLayerInstance = PhotonNetwork.Instantiate(UIPlayerInstance.name, contentTransform.position, contentTransform.rotation, default,null);
            var UIPLayerInstance = Instantiate(UIPlayerInstance, contentTransform);
            UIPLayerInstance.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
            //UIPLayerInstance.transform.SetParent(contentTransform);
            var UIPIController = UIPLayerInstance.GetComponentInChildren<UIPlayerInstanceController>();
            UIPIController.SetUsername(player.NickName);
            UIPIController.SetCbSelectable(PhotonNetwork.LocalPlayer.Equals(player));
            
            //if (player.CustomProperties["IsAttacker"] is null)
            if (!player.CustomProperties.ContainsKey("IsAttacker"))
            {
                Debug.Log($"Players {player.NickName} current state is null");
                continue;
            }
            
            UIPIController.ToggleAttacker((bool) player.CustomProperties["IsAttacker"]);
            UIPIController.ToggleDefender(!(bool) player.CustomProperties["IsAttacker"]);
            Debug.Log($"Players {player.NickName} current state: {(bool) player.CustomProperties["IsAttacker"]}");
        }
    }
    
    [PunRPC]
    public void Refresh()
    {
        Clear();
        Fill();
    }
    #region Unity Methods

    public void Update()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if(player.IsLocal) return;
        }
    }

    #endregion
}
