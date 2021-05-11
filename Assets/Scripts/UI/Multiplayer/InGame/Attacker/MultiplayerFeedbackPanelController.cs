using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MultiplayerFeedbackPanelController : MonoBehaviour
{
    public TextMeshProUGUI ShipCountText;
    public MultiplayerInGameManager multiManager;

    private GameManager _gameManager;
    private PhotonAttackerBehaviour attacker;

    public void RefreshShipCountText()
    {
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        if (!(bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
            return;
        ShipCountText.SetText($"Ships remaining: {multiManager.attacker.shipCount}");
    }

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    #endregion
}
