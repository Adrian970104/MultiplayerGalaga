using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
    public class PhotonBulletBehaviour : MonoBehaviour, IPunObservable
    {
        public int speed = 10;
        public int _damage = 50;
        
        public PhotonView photonView;
        public Vector3 selfDirection;

        private Vector3 _selfPos;
        private static int _maxDistance = 100;


            private void Movement(Vector3 direction)
        {
            transform.position += direction * (speed * Time.deltaTime);
        }

        private void DestroyCheck()
        {
            if (Math.Abs(Vector3.Distance(transform.position, new Vector3(0f, 0f, 0f))) > _maxDistance)
            {
                PhotonNetwork.Destroy(photonView);
            }
        }

        #region Photon Methods

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
            }
            else
            {
                _selfPos = (Vector3)stream.ReceiveNext();
            }
        }
        #endregion
        
        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("AttackerShip"))
            {
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(photonView);
                }
                other.GetComponent<AttackerShipBehaviour>().ChangeHealth(-_damage);
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
                transform.position = Vector3.Lerp(transform.position, _selfPos, Time.deltaTime * 15);
            }

        }

        private void Start()
        {
            _selfPos = transform.position;
        }

        #endregion

    }