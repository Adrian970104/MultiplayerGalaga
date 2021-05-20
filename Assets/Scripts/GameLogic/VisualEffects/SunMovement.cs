using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMovement : MonoBehaviour
{
    void FixedUpdate()
    {
        var point = new Vector3(0,0,0);
        transform.RotateAround(point,transform.up,-0.1f);
    }
}
