﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleplayerInGameCanvasManager : MonoBehaviour
{

    public Canvas InGameCanvas;
    public Canvas WinnerCanvas;

    public TextMeshProUGUI winnerText;

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