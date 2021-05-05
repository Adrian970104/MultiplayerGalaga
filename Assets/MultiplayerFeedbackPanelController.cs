using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MultiplayerFeedbackPanelController : MonoBehaviour
{
    public TextMeshProUGUI ShipCountText;

    public MultiplayerInGameManager multiManager;
    
    private PhotonAttackerBehaviour attacker;

    public void RefreshShipCountText()
    {
        if (!(bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
            return;
        //ShipCountText.SetText($"Ships remaining: {attacker.shipCount}");
        ShipCountText.SetText($"Ships remaining: {multiManager.attacker.shipCount}");
    }

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        if (!(bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
            return;
        
        //attacker = FindObjectOfType<MultiplayerInGameManager>().attacker;
    }

    // Update is called once per frame
    void Update()
    {
        RefreshShipCountText();
    }
    #endregion
}
