using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour
{
    [SerializeField] private LayerMask mask;

    [SerializeField] private Transform point;

    [SerializeField] private float radius;

    public bool CheckDetect()
    {
        return Physics.CheckCapsule(point.position, point.position + new Vector3(0, -0.1f, 0), radius, mask);
       // return Physics.CheckSphere(point.position, radius, mask);
    }

    public LayerMask GetMask()
    {
        return mask;
    }
}
