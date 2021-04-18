using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInstanceController : MonoBehaviour
{
    public TextMeshProUGUI username;
    public Toggle attackerToggle;
    public Toggle defenderToggle;

    private ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();
    private PlayerContainerController _playerContainerController;
    public PhotonView PhotonView;

    public void SetCustProp()
    {
        _myCustomProperties["IsAttacker"] = attackerToggle.isOn;
        PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(_myCustomProperties);
        PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("IsAttacker")
            ? $"Players {PhotonNetwork.LocalPlayer.NickName} IsAttacker successfully set to {(bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"]}"
            : $"Failed to set {PhotonNetwork.LocalPlayer.NickName} players Custom Properties");

        //PhotonView.RPC("Refresh",RpcTarget.All);
        _playerContainerController.photonView.RPC("Refresh",RpcTarget.All);
        //pcc.Update();
    }
    public void SelectedUpdate()
    {
        attackerToggle.onValueChanged.AddListener(delegate
        {
            defenderToggle.isOn = !attackerToggle.isOn;
            //setCustprop();
            //pcc.Fill();
        });
        
        defenderToggle.onValueChanged.AddListener(delegate
        {
            attackerToggle.isOn = !defenderToggle.isOn;
            //setCustprop();
            //pcc.Fill();
        });
    }

    public void SetUsername(string usern)
    {
        username.text = usern;
    }
    
    public void ToggleAttacker(bool isOn)
    {
        attackerToggle.isOn = isOn;
    }
    
    public void ToggleDefender(bool isOn)
    {
        defenderToggle.isOn = isOn;
    }

    public void SetCbSelectable(bool isEnabled)
    {
        attackerToggle.enabled = isEnabled;
        defenderToggle.enabled = isEnabled;
    }
    
    #region Unity Methonds

    public void Start()
    {
        //attackerToggle.isOn = false;
        //defenderToggle.isOn = false;
        PhotonView = gameObject.GetComponent<PhotonView>();
        _playerContainerController = gameObject.GetComponentInParent<PlayerContainerController>();
        attackerToggle.onValueChanged.AddListener(delegate
        {
            defenderToggle.isOn = !attackerToggle.isOn;
            SetCustProp();
        });
        
        defenderToggle.onValueChanged.AddListener(delegate
        {
            attackerToggle.isOn = !defenderToggle.isOn;
            SetCustProp();
        });
    }

    private void Update()
    {
        //SelectedUpdate();
    }

    #endregion
}
