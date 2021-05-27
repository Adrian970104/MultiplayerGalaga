using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MultiplayerFeedbackPanelController : MonoBehaviour
{
    public TextMeshProUGUI ShipCountText;
    public TextMeshProUGUI MaterialText;
    public MultiplayerInGameManager multiManager;

    private GameManager _gameManager;
    private AttackerBehaviour attacker;

    public void RefreshFeedbackPanel()
    {
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        
        if (!(bool) PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"])
            return;
        
        ShipCountText.SetText($"Remaining ships: {multiManager.attacker.shipCount}");
        MaterialText.SetText($"Remaining material: {multiManager.attacker.material}");
    }

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    #endregion
}
