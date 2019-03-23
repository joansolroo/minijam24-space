using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TravelingRigidBody : MonoBehaviour
{

    Rigidbody rb;
    [SerializeField] float forceScale = 1;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        rb.AddForce(CelestialRigidBody.GetForceAt(this.transform.position) * forceScale);
    }
}
