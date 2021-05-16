using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class SingleplayerInGameManager : MonoBehaviour
{
    public GameObject defenderShip;
    
    //TODO Adrián ezt Listával kell megoldani!
    public GameObject attackerShip1;
    public GameObject attackerShip2;
    public GameObject attackerShip3;

    public SingleplayerInGameCanvasManager CanvasManager;
    
    private const int DeadLine = -18;
    private const int VerticalSpeed = 2;
    private const int HorizontalSpeed = 1;
    private Vector3 _verticalDirection = Vector3.right;
    private int _stepCounter = 0;
    
    public List<SpaceShip> attackerShips = new List<SpaceShip>();
    private GameManager _gameManager;
    private Vector3 _defenderPos = new Vector3(0,0,-15);
    private List<Vector3> _attackerStartPos = new List<Vector3>();

    private void AttackerStartPosFill()
    {
        for (var z = 10; z >= 0; z -= 5)
            for (var x = -20; x <= 10; x += 15)
            {
                _attackerStartPos.Add(new Vector3(x,0,z));
            }
    }
    
    private void AttackerSetup()
    {
        var rot = new Quaternion(0,180,0,0);

        var attackers = new[] {attackerShip1, attackerShip2, attackerShip3};
        
        foreach (var pos in _attackerStartPos)
        {
            var shipVariant = Random.Range(0, 3);
            var att = PhotonNetwork.Instantiate(attackers[shipVariant].name, pos, rot);
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
        if (_gameManager.singleplayerPhase != SingleplayerPhase.InGame)
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
        //_gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.AfterGame);
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
    // Start is called before the first frame update
    void Start()
    {
        CanvasManager.SetInGameCanvasVisible();
        
        PhotonNetwork.OfflineMode = true;
        //Need to fix a bug: If coming from multiplayer while no internet!
        PhotonNetwork.Reconnect();

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        
        PhotonNetwork.JoinRandomRoom();
        
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.singleplayerPhase = SingleplayerPhase.InGame;
        
        PhotonNetwork.Instantiate(defenderShip.name, _defenderPos, Quaternion.identity);
        AttackerStartPosFill();
        AttackerSetup();
        
        InvokeRepeating(nameof(InGameMovement), 5.0f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
