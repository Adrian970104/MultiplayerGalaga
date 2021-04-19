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

    private readonly ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();
    private PlayerContainerController _playerContainerController;
    private Button readyButton;

    private void SetCustProp()
    {
        _myCustomProperties["IsAttacker"] = attackerToggle.isOn;
        PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
        PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;
        _playerContainerController.photonView.RPC("Refresh",RpcTarget.All);
    }
    public void SelectedUpdate()
    {
        attackerToggle.onValueChanged.AddListener(delegate
        {
            defenderToggle.isOn = !attackerToggle.isOn;
        });
        
        defenderToggle.onValueChanged.AddListener(delegate
        {
            attackerToggle.isOn = !defenderToggle.isOn;
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

    public void ActivateReadyButton()
    {
        readyButton.interactable = true;
    }
    
    #region Unity Methonds

    public void Start()
    {

        readyButton = GameObject.FindGameObjectWithTag("MultiplayerReadyButton").GetComponent<Button>();
        _playerContainerController = gameObject.GetComponentInParent<PlayerContainerController>();
        attackerToggle.onValueChanged.AddListener(delegate
        {
            defenderToggle.isOn = !attackerToggle.isOn;
            ActivateReadyButton();
            SetCustProp();
        });
        
        defenderToggle.onValueChanged.AddListener(delegate
        {
            attackerToggle.isOn = !defenderToggle.isOn;
            ActivateReadyButton();
            SetCustProp();
        });
    }
    
    #endregion
}
