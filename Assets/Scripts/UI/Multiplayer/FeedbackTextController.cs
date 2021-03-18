using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackTextController : MonoBehaviour
{
    public TextMeshProUGUI feedback;
    public void SetFeedbackText(string text, Color color)
    {
        feedback.text = text;
        feedback.color = color;
    }
}
