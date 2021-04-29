using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerInGameManager : MonoBehaviour
{

    public Canvas attackerCanvas;
    public Canvas defenderCanvas;

    public GameObject defenderPrefab;
    public GameObject attackerPrefab;

    private GameManager _gameManager;
    private Vector3 _attackerInstPos = new Vector3(20f,20f,20f);
    private readonly Vector3 _defenderInstPos = new Vector3(0f,0f,-15f);
    // Start is called before the first frame update
    void Start()
    {
        var buttonHandler = FindObjectOfType<MultiplayerInGameButtonHandler>();
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.multiplayerPhase = MultiplayerPhase.InDeploy;
        
        var isAttacker  = (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAttacker"];
        attackerCanvas.enabled = isAttacker;
        defenderCanvas.enabled = !isAttacker;

        if (isAttacker)
        {
            buttonHandler.attacker = PhotonNetwork.Instantiate(attackerPrefab.name, _attackerInstPos, Quaternion.identity).GetComponent<PhotonAttackerBehaviour>();
        }
        else
        {
            PhotonNetwork.Instantiate(defenderPrefab.name, _defenderInstPos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
