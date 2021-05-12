using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class PhotonAttackerBehaviour : MonoBehaviour
{
    public GameObject shipToDeploy;
    public int shipCount = 5;
    public int material = 1500;
    public List<AttackerShipBehaviour> attackerShips = new List<AttackerShipBehaviour>();

    private const int VerticalSpeed = 2;
    private const int HorizontalSpeed = 1;
    private GameManager _gameManager;
    private MultiplayerInGameManager _multiManager;
    private Vector3 _verticalDirection = Vector3.right;
    private int _stepCounter = 0;
    private MultiplayerFeedbackPanelController _feedbackPanelController;
    
    private readonly int _leftBorder = -20;
    private readonly int _rightBorder = 8;
    private readonly int _upBorder = 14;
    private readonly int _downBorder = -3;

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
            shipBehav.attackerPlayer = gameObject.GetComponent<PhotonAttackerBehaviour>();
            attackerShips.Add(shipBehav);
            shipToDeploy = null;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if(shipToDeploy is null)
                return;
            
            PhotonNetwork.Destroy(shipToDeploy);
            shipToDeploy = null;
            ++shipCount;
            _feedbackPanelController.RefreshFeedbackPanel();
        }
    }

    private void DeployMovement(GameObject ship)
    {
        var translationV = Input.GetAxis("Vertical") * 10.0f * -1 * Time.deltaTime;
        var translationH = Input.GetAxis("Horizontal") * 10.0f * -1 * Time.deltaTime;
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

    public void InGameStep(Vector3 direction, int speed)
    {
        foreach (var ship in attackerShips)
        {
             ship.transform.position += direction * speed;
        }
    }

    public void InGameMovement()
    {
        if (_gameManager.multiplayerPhase != MultiplayerPhase.InGame)
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
            return;
        }
        
        InGameStep(_verticalDirection, VerticalSpeed);
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
        
        if(_gameManager.multiplayerPhase != MultiplayerPhase.InGame)
            return;

        _gameManager.photonView.RPC("SetMultiplayerPhase",RpcTarget.All,MultiplayerPhase.InDeploy);
        shipCount = 5;
        _feedbackPanelController.RefreshFeedbackPanel();
    }

    public void EndCheck()
    {
        if(material <= 0)
            Defeated();
    }

    public void Defeated()
    {
        if(PhotonNetwork.PlayerListOthers.Length < 1)
            return;
        var winner = !(bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"] ? PhotonNetwork.LocalPlayer.NickName : PhotonNetwork.PlayerListOthers[0].NickName;
        _multiManager.photonView.RPC("EndMultiplayer", RpcTarget.All, winner);
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
        InvokeRepeating(nameof(InGameMovement), 5.0f, 2.0f);
    }
    #endregion
    
}
