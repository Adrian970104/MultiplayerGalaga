using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
    public class PhotonBulletBehaviour : MonoBehaviour
    {
        public PhotonView photonView;
        public int speed = 10;

        public Vector3 selfDirection;

        
        public void Movement(Vector3 direction)
        {
            transform.position += direction * (speed * Time.deltaTime);
        }
        
        #region Unity Methods

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                Movement(selfDirection);
            }
        }
        #endregion
        
    }