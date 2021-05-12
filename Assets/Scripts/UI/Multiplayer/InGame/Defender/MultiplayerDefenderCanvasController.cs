using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerDefenderCanvasController : MonoBehaviour
{
    public MultiplayerDefenderHealthPanelController hpController;
    public MultiplayerDefenderDataPanelController dataController;

    public void RefreshHealthPanel(int actualHealth, int maxHealth)
    {
        hpController.Refresh(actualHealth,maxHealth);
    }

    public void RefreshDataPanel(int actualDamage)
    {
        dataController.Refresh(actualDamage);
    }
}
