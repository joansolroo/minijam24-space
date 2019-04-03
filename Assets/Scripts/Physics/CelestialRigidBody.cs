using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CelestialRigidBody : MonoBehaviour
{
    public static List<CelestialRigidBody> celestialBodies = new List<CelestialRigidBody>();
    // Start is called before the first frame update

    public Rigidbody rb;
    public CelestialRigidBody parent;

    Ellipse orbit;
    [SerializeField] Vector2 orbitSize = Vector2.one;
    void Awake()
    {
        celestialBodies.Add(this);
        rb = GetComponent<Rigidbody>();
        orbit = new Ellipse();

        if (parent != null)
        {
            Vector3 radius = this.transform.position - parent.transform.position;
            float r = radius.magnitude;
            orbit.center = parent.transform.position;
            orbit.size = new Vector2(orbitSize.x, orbitSize.y) * r;

            transform.position = orbit.Evaluate(0);
        }
    }

    [SerializeField] float tYear = 0;
    [SerializeField] float tDay = 0;
    [SerializeField] float yearPeriod = 1;
    [SerializeField] float dayPeriod = 1;

    private void FixedUpdate()
    {
        tYear = (tYear + Time.deltaTime / yearPeriod) % 1;
        Vector3 newPos = orbit.Evaluate(tYear);
        rb.velocity = (newPos - transform.position) / Time.deltaTime;

        tDay = (tDay + Time.deltaTime / dayPeriod) % 1;
        Vector3 rotation = this.transform.localEulerAngles;
        rotation.y = 360 * tDay;
        this.transform.localEulerAngles = rotation;

        /*if (parent != null)
        {
            Vector3 radius = this.transform.position - parent.transform.position;
            Vector3 tangent = new Vector3(-radius.z, 0, radius.x);
            r = radius.magnitude;
            Vector3 velocity = tangent;
            velocity.Scale(tangential);
            rb.velocity = velocity ;
            rb.AddForce(radius*(-normal));
        }*/
    }

    /* Follow fixed trajectory
    private void FixedUpdate()
    {
        t += Time.deltaTime / period;
        t %= 1;
        Vector3 newPos = orbit.Evaluate(t);
        rb.velocity = (newPos-transform.position)/Time.deltaTime;
    }
    */
    private void OnDrawGizmos()
    {

        if (orbit != null)
        {
            Gizmos.color = Color.gray;
            Utils.GizmosExtensions.DrawWireEllipse(orbit.center, orbit.size, 80, Quaternion.identity);

            for (int i = 0; i < 20; ++i)
            {
                Gizmos.DrawSphere(orbit.Evaluate(i / 20f), 0.1f);
            }
            if (parent != null)
            {

                Ellipse currentOrbit = PhysicsSpace.ComputeOrbit(this.rb, parent.rb);
                Gizmos.color = Color.red;
                //Utils.GizmosExtensions.DrawWireEllipse(currentOrbit.center, currentOrbit.size, 20, Quaternion.identity);


                Gizmos.DrawLine(this.transform.position, parent.transform.position);

                int count = 100;
                for (int i = 0; i < count; ++i)
                {
                    float t = ((float)i) / count;
                    Gizmos.DrawLine(orbit.Evaluate(t), currentOrbit.Evaluate(t));
                }
            }
        }
        if (rb != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + rb.velocity.normalized);
        }
    }
}
