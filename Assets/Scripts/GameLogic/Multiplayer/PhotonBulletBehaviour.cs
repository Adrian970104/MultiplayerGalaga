using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
    public class PhotonBulletBehaviour : MonoBehaviour, IPunObservable
    {
        public PhotonView photonView;
        public int speed = 10;
        public Vector3 selfDirection;

        private Vector3 _selfPos;

        
        public void Movement(Vector3 direction)
        {
            transform.position += direction * (speed * Time.deltaTime);
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

        private void Update()
        {
            if (photonView.IsMine)
            {
                Movement(selfDirection);
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