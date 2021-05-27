using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerInGameCanvasManager : MonoBehaviour
{
    
    public Canvas InGameCanvas;
    public SingleplayerHealthPanelController HealthPanelController;
    public SingleplayerDataPanelController DataPanelController;
    
    public Canvas WinnerCanvas;
    public TextMeshProUGUI winnerText;


    public void RefreshHealthPanel(int actualHealth, int maxHealth)
    {
        HealthPanelController.Refresh(actualHealth, maxHealth);
    }
    public void RefreshDataPanel(int actualDam, int score)
    {
        DataPanelController.Refresh(actualDam, score);
        //DataPanelController.RefreshScore(score);
    }
    public void FillWinnerCanvas(bool win)
    {
        var text = win ? "Congratulation!" : "Loose!";
        winnerText.SetText(text);
    }

    public void SetWinnerCanvasVisible()
    {
        InGameCanvas.enabled = false;
        WinnerCanvas.enabled = true;
    }

    public void SetInGameCanvasVisible()
    {
        WinnerCanvas.enabled = false;
        InGameCanvas.enabled = true;
    }
}
