using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonAttackerBehaviour : MonoBehaviour
{
    public GameObject shipToDeploy;
    
    public void Deploy()
    {
        if(shipToDeploy is null) return;
        
        MoveShip(shipToDeploy);

        if (Input.GetKey(KeyCode.Return))
        {
            shipToDeploy = null;
        }
    }

    private void MoveShip(GameObject ship)
    {
        var translationV = Input.GetAxis("Vertical") * 10.0f * Time.deltaTime;
        var translationH = Input.GetAxis("Horizontal") * 10.0f * Time.deltaTime;
        ship.transform.Translate(translationH, 0, translationV);
    }

    #region Unity Methods

    void Update()
    {
        Deploy();
    }

    void Start()
    {
        shipToDeploy = null;
    }
    #endregion
    
}
