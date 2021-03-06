using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class SingleplayerInGameManager : MonoBehaviour
{
    public GameObject defenderShip;
    public List<GameObject> attackerShipVariants = new List<GameObject>();
    public SingleplayerInGameCanvasManager CanvasManager;
    
    private const int DeadLine = -39;
    private const int VerticalSpeed = 4;
    private const int HorizontalSpeed = 2;
    private Vector3 _verticalDirection = Vector3.right;
    private int _stepCounter = 0;
    
    public List<SpaceShip> attackerShips = new List<SpaceShip>();
    private GameManager _gameManager;
    private Vector3 _defenderPos = new Vector3(0,0,-25);
    private List<Vector3> _attackerStartPos = new List<Vector3>();

    private void AttackerStartPosFill()
    {
        for (var z = 20; z >= 0; z -= 10)
            for (var x = -55f; x <= 15f; x += 17.5f)
            {
                _attackerStartPos.Add(new Vector3(x,0,z));
            }
    }
    
    private void AttackerSetup()
    {
        var rot = new Quaternion(0,180,0,0);

        foreach (var pos in _attackerStartPos)
        {
            var shipVariant = Random.Range(0, 3);
            var att = PhotonNetwork.Instantiate(attackerShipVariants[shipVariant].name, pos, rot);
            att.GetComponent<AttackerShipBehaviour>().singleManager = this;
            attackerShips.Add(att.GetComponent<SpaceShip>());
        }
    }

    private void InGameStep(Vector3 direction, int speed)
    {
        foreach (var ship in attackerShips)
        {
            if(ship is null)
                return;
            ship.transform.position += direction * speed;
        }
    }

    public void InGameMovement()
    {
        if (_gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
        {
            return;
        }

        if (_stepCounter < 4)
        {
            InGameStep(_verticalDirection, VerticalSpeed);
            _stepCounter++;
            return;
        }
        
        if (_stepCounter == 4)
        {
            InGameStep(_verticalDirection, VerticalSpeed);
            _stepCounter = 10;
            return;
        }

        if (_stepCounter % 10 == 0)
        {
            InGameStep(Vector3.back, HorizontalSpeed);
            ChangeVerticalDirection();
            _stepCounter++;
            EndCheck();
            return;
        }
        
        InGameStep(_verticalDirection, VerticalSpeed);
        _stepCounter++;
    }
    
    private void ChangeVerticalDirection()
    {
        _verticalDirection = _verticalDirection == Vector3.right ? Vector3.left : Vector3.right;
    }
    
    public void EndSingleplayer(bool win)
    {
        _gameManager.singleplayerPhase = SingleplayerPhase.AfterGame;
        CanvasManager.FillWinnerCanvas(win);
        CanvasManager.SetWinnerCanvasVisible();
    }

    public void EndCheck()
    {
        if (attackerShips.Count <= 0)
        {
            EndSingleplayer(true);
            return;
        }
        
        if (attackerShips.Any(attacker => attacker.transform.position.z <= DeadLine))
        {
            EndSingleplayer(false);
        }
    }

    #region Unity Methods
    void Start()
    {
        CanvasManager.SetInGameCanvasVisible();
        
        if(PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
        
        PhotonNetwork.OfflineMode = true;

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        
        PhotonNetwork.JoinRandomRoom();
        
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.singleplayerPhase = SingleplayerPhase.InAttack;
        
        PhotonNetwork.Instantiate(defenderShip.name, _defenderPos, Quaternion.identity);
        AttackerStartPosFill();
        AttackerSetup();
        
        InvokeRepeating(nameof(InGameMovement), 5.0f, 2.0f);
    }
    #endregion
}
