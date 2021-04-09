using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;

public class PlayerContainerController : MonoBehaviour
{
    [SerializeField]
    public GameObject UIPlayerInstance;

    public PhotonView photonView;
    public GameObject content;

    public void Clear()
    {
        var UIPlayers = GameObject.FindGameObjectsWithTag("UIPlayerInstance");
        foreach (var UIPlayer in UIPlayers)
        {
            Debug.Log($"Player Ui instance destroyed: {UIPlayer}");
            Destroy(UIPlayer);
        }
    }

    public void Fill()
    {
        //var canvas =  GameObject.FindGameObjectWithTag("PlayerContainerCanvas");
        //var canvas = gameObject.GetComponentInChildren<>("Content");
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            //if(photonView.Is)
            Debug.Log($"Player Ui instance created: {player.NickName}");
            //var canvasTransform = canvas.transform;
            var contentTransform = content.transform;
            var UIPLayerInstance = PhotonNetwork.Instantiate(UIPlayerInstance.name, contentTransform.position, contentTransform.rotation, default,null);
            UIPLayerInstance.transform.SetParent(contentTransform);
            //UIPLayerInstance.transform.localScale = new Vector3((float)0.5,(float)0.25,(float)0.5);
            var UIPIController = UIPLayerInstance.GetComponentInChildren<UIPlayerInstanceController>();
            UIPIController.SetUsername(player.NickName);
            UIPIController.SetCbSelectable(PhotonNetwork.LocalPlayer.Equals(player));
        }
    }
    
    public void Refresh()
    {
        Clear();
        Fill();
    }
    #region Unity Methods
    #endregion
}
