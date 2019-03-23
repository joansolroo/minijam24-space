using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CelestialRigidBody : MonoBehaviour
{
    static List<CelestialRigidBody> bodies = new List<CelestialRigidBody>();
    // Start is called before the first frame update

    Rigidbody rb;

    void Awake()
    {
        bodies.Add(this);
        rb = GetComponent<Rigidbody>();
    }

    public static Vector3 GetForceAt(Vector3 universePosition)
    {
        Vector3 force = Vector3.zero;
        foreach(CelestialRigidBody body in bodies)
        {
            Vector3 direction = body.transform.position- universePosition;
            force += direction.normalized/direction.magnitude * body.rb.mass*Time.unscaledDeltaTime*100;
        }

        return force;
    }
}
