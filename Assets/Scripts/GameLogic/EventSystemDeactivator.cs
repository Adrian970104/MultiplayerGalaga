using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.EventSystems;

public class EventSystemDeactivator : MonoBehaviour
{
    // Start is called before the first frame update
    private EventSystem esm;
    void Start()
    {
        esm = GetComponent<EventSystem>();
    }

    private void Update()
    {
        esm.SetSelectedGameObject(null, null);
    }
}
