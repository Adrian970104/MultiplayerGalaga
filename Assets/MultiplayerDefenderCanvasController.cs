using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerDefenderCanvasController : MonoBehaviour
{
    public MultiplayerDefenderHealthPanelController hpController;

    public void RefreshHealthPanel(int actualHealth, int maxHealth)
    {
        hpController.Refresh(actualHealth,maxHealth);
    }
}
