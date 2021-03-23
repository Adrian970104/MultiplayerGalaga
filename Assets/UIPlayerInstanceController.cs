using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInstanceController : MonoBehaviour
{
    public TextMeshProUGUI username;
    public Toggle attackerToggle;
    public Toggle defenderToggle;


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

    public void setActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    
    #region Unity Methond

    private void Start()
    {
    }

    #endregion
}
