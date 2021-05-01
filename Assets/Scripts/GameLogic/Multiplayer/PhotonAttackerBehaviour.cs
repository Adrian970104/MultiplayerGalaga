using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonAttackerBehaviour : MonoBehaviour
{
    public GameObject shipToDeploy;
    
    private readonly int _leftBorder = -28;
    private readonly int _rightBorder = 28;
    private readonly int _upBorder = 15;
    private readonly int _downBorder = -10;

    public void Deploy()
    {
        if(shipToDeploy is null) return;
        
        MoveShip(shipToDeploy);
        BorderCheck(shipToDeploy);
        if (Input.GetKey(KeyCode.Return))
        {
            /*if(Math.Abs(Vector3.Distance(shipToDeploy.transform.position,_deployPos)) < 10)
                return;*/
            
            var shipBehav = shipToDeploy.GetComponent<AttackerShipBehaviour>();
            if(shipBehav.triggerCount > 0)
                return;
            
            shipBehav.isDeployed = true;
            shipToDeploy = null;
        }
    }

    private void MoveShip(GameObject ship)
    {
        var translationV = Input.GetAxis("Vertical") * 10.0f * Time.deltaTime;
        var translationH = Input.GetAxis("Horizontal") * 10.0f * Time.deltaTime;
        ship.transform.Translate(translationH, 0, translationV);
    }

    private void BorderCheck(GameObject ship)
    {
        var pos = ship.transform.position;
        
        if (pos.x > _rightBorder)
        {
            ship.transform.position = new Vector3(_rightBorder,pos.y,pos.z);
        }
        
        if (pos.x < _leftBorder)
        {
            ship.transform.position = new Vector3(_leftBorder, pos.y, pos.z);
        }
        
        if (pos.z > _upBorder)
        {
            ship.transform.position = new Vector3(pos.x, pos.y, _upBorder);
        }
        
        if (pos.z < _downBorder)
        {
            ship.transform.position = new Vector3(pos.x,pos.y,_downBorder);
        }
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
