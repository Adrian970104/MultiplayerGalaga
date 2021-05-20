using Photon.Pun;
using UnityEngine;

public class ExplosionCleanup : MonoBehaviour
{
    public PhotonView photonView;
    private float initializationTime;
    [SerializeField] private int _alivetime = 3;
    
    public void Start()
    {
        initializationTime = Time.timeSinceLevelLoad;
    }

    public void Update()
    {
        var timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;

        if (timeSinceInitialization < _alivetime) 
            return;
        
        if(photonView.IsMine)
            PhotonNetwork.Destroy(photonView);
    }
}
