using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MultiplayerInGameCanvasManager : MonoBehaviour
{
    public Canvas attackerCanvas;
    public Canvas defenderCanvas;
    public Canvas winnerCanvas;

    public TextMeshProUGUI winnerText;

    public void SetAttackerCanvasVisible()
    {
        defenderCanvas.enabled = false;
        winnerCanvas.enabled = false;
        attackerCanvas.enabled = true;
    }
    
    public void SetDefenderCanvasVisible()
    {
        attackerCanvas.enabled = false;
        winnerCanvas.enabled = false;
        defenderCanvas.enabled = true;
    }

    public void FillWinnerCanvas(string winner)
    {
        winnerText.SetText($"The winner is: {winner}");
    }
    
    public void SetWinnerCanvasVisible()
    {
        attackerCanvas.enabled = false;
        defenderCanvas.enabled = false;
        winnerCanvas.enabled = true;
    }
}
