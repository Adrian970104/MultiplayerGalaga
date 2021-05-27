using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class AttackerBehaviour : MonoBehaviour
{
    public GameObject shipToDeploy;
    public int shipCount = 5;
    public int material = 1500;
    public List<AttackerShipBehaviour> attackerShips = new List<AttackerShipBehaviour>();

    private GameManager _gameManager;
    private MultiplayerInGameManager _multiManager;
    private MultiplayerFeedbackPanelController _feedbackPanelController;
    private const int VerticalSpeed = 4;
    private const int HorizontalSpeed = 2;
    private Vector3 _verticalDirection = Vector3.right;
    private int _stepCounter;
    
    
    private const float _deployMovementSpeed = 15;
    private const int _deadLine = -39;
    private readonly int _leftBorder = -58;
    private readonly int _rightBorder = 17;
    private readonly int _upBorder = 25;
    private readonly int _downBorder = -16;

    public void Deploy()
    {
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InDeploy)
            return;
        if(shipToDeploy is null) 
            return;
        
        DeployMovement(shipToDeploy);
        BorderCheck(shipToDeploy);
        
        if (Input.GetKey(KeyCode.Return))
        {
            var shipBehav = shipToDeploy.GetComponent<AttackerShipBehaviour>();

            if(shipBehav.triggerCount > 0)
                return;
            
            shipBehav.isDeployed = true;
            shipBehav.attackerPlayer = gameObject.GetComponent<AttackerBehaviour>();
            attackerShips.Add(shipBehav);
            shipToDeploy = null;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if(shipToDeploy is null)
                return;
            
            PhotonNetwork.Destroy(shipToDeploy);
            ++shipCount;
            material += shipToDeploy.GetComponent<AttackerShipBehaviour>().cost;
            shipToDeploy = null;
            _feedbackPanelController.RefreshFeedbackPanel();
        }
    }

    private void DeployMovement(GameObject ship)
    {
        var translationV = Input.GetAxis("Vertical") * _deployMovementSpeed * -1 * Time.deltaTime;
        var translationH = Input.GetAxis("Horizontal") * _deployMovementSpeed * -1 * Time.deltaTime;
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

    private void InAttackStep(Vector3 direction, int speed)
    {
        foreach (var ship in attackerShips)
        {
             ship.transform.position += direction * speed;
        }
    }

    public void InAttackMovement()
    {
        if (_gameManager.multiplayerPhase != MultiplayerPhase.InAttack)
        {
            return;
        }

        if (_stepCounter < 4)
        {
            InAttackStep(_verticalDirection, VerticalSpeed);
            _stepCounter++;
            return;
        }
        
        if (_stepCounter == 4)
        {
            InAttackStep(_verticalDirection, VerticalSpeed);
            _stepCounter = 10;
            return;
        }

        if (_stepCounter % 10 == 0)
        {
            InAttackStep(Vector3.back, HorizontalSpeed);
            ChangeVerticalDirection();
            _stepCounter++;
            EndCheck();
            return;
        }
        
        InAttackStep(_verticalDirection, VerticalSpeed);
        _stepCounter++;
    }

    private void ChangeVerticalDirection()
    {
        _verticalDirection = _verticalDirection == Vector3.right ? Vector3.left : Vector3.right;
    }

    public void WaveEndCheck()
    {
        if (attackerShips.Count == 0)
        {
            EndWave();
        }
    }

    public void EndWave()
    {
        EndCheck();
        
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InAttack)
            return;

        _gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.InDeploy);
        shipCount = 5;
        _feedbackPanelController.RefreshFeedbackPanel();
        _stepCounter = 0;
        _verticalDirection = Vector3.right;
    }

    public void EndCheck()
    {
        if (material <= 0 && attackerShips.Count == 0)
            Ended(false);
        if(attackerShips.Any(attacker => attacker.transform.position.z <= _deadLine))
            Ended(true);
    }

    public void Defeated()
    {
        if(PhotonNetwork.PlayerListOthers.Length < 1)
            return;
        var winner = !(bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"] ? PhotonNetwork.LocalPlayer.NickName : PhotonNetwork.PlayerListOthers[0].NickName;
        _multiManager.photonView.RPC("EndMultiplayer", RpcTarget.All, winner);
    }

    public void Ended(bool win)
    {
        Debug.Log("Az attacker behav hívta az End-et!");
        if(PhotonNetwork.PlayerListOthers.Length < 1)
            return;

        if (win)
        {
            var winner = (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"] ? PhotonNetwork.LocalPlayer.NickName : PhotonNetwork.PlayerListOthers[0].NickName;
            _multiManager.photonView.RPC("EndMultiplayer", RpcTarget.All, winner);
        }

        if (!win)
        {
            var winner = !(bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"] ? PhotonNetwork.LocalPlayer.NickName : PhotonNetwork.PlayerListOthers[0].NickName;
            _multiManager.photonView.RPC("EndMultiplayer", RpcTarget.All, winner);
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
        _gameManager = FindObjectOfType<GameManager>();
        _multiManager = FindObjectOfType<MultiplayerInGameManager>();
        _feedbackPanelController = FindObjectOfType<MultiplayerFeedbackPanelController>();
        _feedbackPanelController.RefreshFeedbackPanel();
        InvokeRepeating(nameof(InAttackMovement), 5.0f, 2.0f);
    }
    #endregion
    
}
