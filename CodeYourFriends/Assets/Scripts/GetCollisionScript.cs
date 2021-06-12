using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCollisionScript : MonoBehaviour
{
    [SerializeField] 
    private LayerMask layerMask;

    public bool IsColliding
    {
        get;
        private set;
    }

    private void OnTriggerStay(Collider other)
    {
        IsColliding = other != null && ((1 << other.gameObject.layer) & layerMask) != 0;
    }

    private void OnTriggerExit(Collider other)
    {
        IsColliding = false;
    }
}
