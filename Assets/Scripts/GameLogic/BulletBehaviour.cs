using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class BulletBehaviour : SpaceObject
    {
        public int damage = 50;
        public string ownerTag;
        private GameManager _gameManager;

        [PunRPC]
        public void SetColor(float r, float g, float b)
        {
            GetComponent<Renderer>().material.color = new Color(r,g,b);
        }
        
        [PunRPC]
        public void SetOwnerTag(string newTag)
        {
            ownerTag = newTag;
        }
        
        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            if(_gameManager is null)
                return;
            if(_gameManager.multiplayerPhase != MultiplayerPhase.InAttack && _gameManager.singleplayerPhase != SingleplayerPhase.InAttack)
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

                attacker.photonView.RPC("TakeDamage",RpcTarget.All,damage);
                
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(photonView);
                }
            }

            if (other.CompareTag("DefenderShip"))
            {
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(photonView);
                }
                other.GetComponentInParent<DefenderShipBehaviour>().photonView.RPC("TakeDamage",RpcTarget.All,damage);
            }
        }
        
        public override void Start()
        {
            base.Start();
            _gameManager = FindObjectOfType<GameManager>();
        }
        

        #endregion

    }