using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using WebSocketSharp;

public class PhotonBulletBehaviour : MonoBehaviour, IPunObservable
    {
        public int speed = 6;
        public int _damage = 50;
        public string ownerTag;
        
        public PhotonView photonView;
        public Vector3 selfDirection;

        private Vector3 _selfPos;
        private Vector3 _starPos;
        private static int _maxDistance = 50;
        private GameManager _gameManager;
        private float _lag;


        private void Movement(Vector3 direction)
        {
            transform.position += direction * (speed * Time.deltaTime);
        }

        private void DestroyCheck()
        {
            if (Math.Abs(Vector3.Distance(transform.position, _starPos)) > _maxDistance)
            {
                PhotonNetwork.Destroy(photonView);
            }
        }
        
        #region Photon Methods

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(ownerTag);
                stream.SendNext(transform.position);
            }
            else
            {
                ownerTag = (string) stream.ReceiveNext();
                _selfPos = (Vector3) stream.ReceiveNext();
                _lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
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
            
            if (other.CompareTag("AttackerShip") || other.CompareTag("DefenderShip"))
            {
                var attacker = other.GetComponentInParent<AttackerShipBehaviour>();
                if (attacker != null)
                {
                    if(!attacker.isDeployed)
                        return;
                }
                
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(photonView);
                }
                other.GetComponentInParent<SpaceShip>().TakeDamage(_damage);
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                Movement(selfDirection);
                DestroyCheck();
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _selfPos, Time.deltaTime*10 + _lag);
            }
        }

        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _selfPos = transform.position;
            _starPos = _selfPos;
        }

        #endregion

    }