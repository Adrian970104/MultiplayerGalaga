using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using WebSocketSharp;

public class PhotonBulletBehaviour : SpaceObject
    {
        public int _damage = 50;
        public string ownerTag;
        private GameManager _gameManager;
        
        #region Photon Methods
        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            base.OnPhotonSerializeView(stream, info);
            if (stream.IsWriting)
            {
                stream.SendNext(ownerTag);
            }
            else
            {
                ownerTag = (string) stream.ReceiveNext();
            }
        }
        #endregion
        
        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            if(_gameManager is null)
                return;
            if(_gameManager.multiplayerPhase != MultiplayerPhase.InGame && _gameManager.singleplayerPhase != SingleplayerPhase.InGame)
                return;
            if(!photonView.IsMine)
                return;
            if(ownerTag.IsNullOrEmpty())
                return;
            if (other.CompareTag(ownerTag))
                return;

            if (other.CompareTag("AttackerShip"))
            {
                var attacker = other.GetComponentInParent<AttackerShipBehaviour>();
                
                if(!attacker.isDeployed)
                        return;
                
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(photonView);
                }
                
                attacker.TakeDamage(_damage);
            }

            if (other.CompareTag("DefenderShip"))
            {
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(photonView);
                }
                //other.GetComponentInParent<DefenderShipBehaviour>().TakeDamage(_damage);
                other.GetComponentInParent<DefenderShipBehaviour>().photonView.RPC("TakeDamage",RpcTarget.All,_damage);
            }
        }
        
        public override void Start()
        {
            base.Start();
            _gameManager = FindObjectOfType<GameManager>();
        }
        

        #endregion

    }