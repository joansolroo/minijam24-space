using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iTrajectory
{

}

/*
// TODO implement this: http://www.braeunig.us/space/orbmech.htm
public class OrbitTrajectory : iTrajectory
{
    static double G = 6.67259e-11;// N-m2/kg2;
    float semiMajorAxis;
    float Eccentricity;
    float Inclination;
    float Periapsis; //angular velocity, periapsis = 2pi/period
    float PeriapsisTime;
    float AscendingLongitude;

    float Period;
    float Anomaly;

    float r; // orbit radius small thingy
    float R; // orbit radius big thingy
    float m; //mass small thingy
    float M; //mass big thingy
    float center;// causes MR= mr => m/M = R/r;

    float period() { return Mathf.Sqrt((float)(4 * Mathf.PI * Mathf.Pow(r, 3) / (G * M))); 

}
    */

public class Sphere
{
    public static Vector3 Evaluate(float u, float v)
    {
        float angleA = u/2 * Mathf.PI * 2;
        float angleB = v * Mathf.PI * 2;
        Vector3 p = new Vector3(Mathf.Sin(angleA) * Mathf.Cos(angleB), Mathf.Cos(angleA), Mathf.Sin(angleA) * Mathf.Sin(angleB)) * 0.5f;
        return p;
    }
    public static Vector3 Evaluate(Vector3 position, Vector3 scale, float u, float v)
    {
        float angleA = u/2 * Mathf.PI * 2;
        float angleB = v * Mathf.PI * 2;
        Vector3 p = new Vector3(Mathf.Sin(angleA) * Mathf.Cos(angleB), Mathf.Cos(angleA), Mathf.Sin(angleA) * Mathf.Sin(angleB)) * 0.5f;
        p = Vector3.Scale(p, scale) + position;
        return p;
    }
}
public class Ellipse : iTrajectory
{
    public Vector3 center;
    public Vector2 size;

    public Vector3 Evaluate(float t)
    {
        return Evaluate(center, this.size, t);
    }

    public static Vector3 Evaluate(Vector3 position, Vector2 shape, float t)
    {
        float angle = t * Mathf.PI * 2;
        return new Vector3(Mathf.Sin(angle) * shape.x, 0, Mathf.Cos(angle) * shape.y) + position;
    }

}

public class Trajectory : iTrajectory
{
    public Vector3[] path;
    public float[] time;
    public bool collision;
}



[ExecuteInEditMode]
public class PhysicsSpace : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float trajectoryTimeScaleFactor = 2;

    static PhysicsSpace instance;

    void Awake()
    {
        instance = this;
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float radius = this.transform.lossyScale.magnitude / 2;
        float massSqrt = 10;// Mathf.Pow(rb.mass, 1 / 8f);
        for (float r = 0.5f; r < massSqrt * 4; r += massSqrt * 0.05f)
        {
            Vector3 p0 = Vector3.zero;
            bool init = false;
            for (int u = 0; u <= 360; u += 5)
            {
                if (!init)
                {
                    p0 = this.transform.TransformPoint(r * Mathf.Cos(u * Mathf.Deg2Rad), 0, r * Mathf.Sin(u * Mathf.Deg2Rad));
                    Vector3 f = GetForceAt(p0);
                    p0.y -= f.magnitude * 0.01f;
                    init = true;
                }
                else
                {
                    Vector3 p1 = this.transform.TransformPoint(r * Mathf.Cos(u * Mathf.Deg2Rad), 0, r * Mathf.Sin(u * Mathf.Deg2Rad));
                    Vector3 f = GetForceAt(p1);
                    p1.y -= f.magnitude * 0.01f;
                    //Gizmos.DrawWireSphere(p1, f.magnitude * 0.05f);

                    Gizmos.color = new Color(0, 1, 1, f.magnitude * 0.01f);
                    Gizmos.DrawLine(p0, p1);
                    p0 = p1;
                }
            }
        }
        for (int u = 0; u <= 360; u += 5)
        {
            Vector3 p0 = Vector3.zero;
            bool init = false;
            for (float r = 0.5f; r < massSqrt * 4; r += massSqrt * 0.05f)
            {
                if (!init)
                {
                    p0 = this.transform.TransformPoint(r * Mathf.Cos(u * Mathf.Deg2Rad), 0, r * Mathf.Sin(u * Mathf.Deg2Rad));
                    Vector3 f = GetForceAt(p0);
                    p0.y -= f.magnitude * 0.01f;
                    init = true;
                }
                else
                {
                    Vector3 p1 = this.transform.TransformPoint(r * Mathf.Cos(u * Mathf.Deg2Rad), 0, r * Mathf.Sin(u * Mathf.Deg2Rad));
                    Vector3 f = GetForceAt(p1);
                    p1.y -= f.magnitude * 0.01f;
                    //Gizmos.DrawWireSphere(p1, f.magnitude * 0.05f);

                    Gizmos.color = new Color(0, 1, 1, f.magnitude * 0.01f);
                    Gizmos.DrawLine(p0, p1);
                    p0 = p1;
                }
            }
        }
    }*/

    public static Trajectory ComputeTrajectory(Rigidbody rb, float time, LayerMask layerMask, float safeDiameter, float forceScale)
    {
        Trajectory trajectory = new Trajectory();

        Transform transform = rb.transform;
        float duration = 10;
        float step = Time.deltaTime * instance.trajectoryTimeScaleFactor;
        int collisionTest = (int)(10 / Time.timeScale);
        Vector3 pos = transform.position;
        Vector3 velocity = rb.velocity;


        List<Vector3> points = new List<Vector3>();
        points.Add(pos);
        bool collision = false;
        int i = 0;
        for (float t = 0; !collision && t < duration; t += step)
        {

            Vector3 newPos = pos + velocity * step;
            Vector3 forceAtnewPos = PhysicsSpace.GetForceAt(newPos) * forceScale * Time.unscaledDeltaTime * 100;

            pos = newPos;
            velocity = velocity + forceAtnewPos;

            //crash prediction
            i++;
            if (i % collisionTest == 0)
            {
                collision = Physics.OverlapSphere(newPos, safeDiameter, layerMask).Length > 0;
            }
            points.Add(newPos);
        }
        trajectory.collision = collision;
        trajectory.path = points.ToArray();
        return trajectory;
    }

    public static Ellipse ComputeOrbit(Rigidbody child, Rigidbody parent)
    {
        Ellipse result = new Ellipse();
        result.center = parent.transform.position;

        // Vector from parent to child
        Vector3 delta = child.transform.position - parent.transform.position;
        float angleCenter = Vector3.Angle(delta, Vector3.forward) * Mathf.Deg2Rad;

        // Angle difference with curve tangent
        float angleTangent = (Vector3.Angle(child.velocity, delta) - 90) * Mathf.Deg2Rad;
        float tangent = Mathf.Tan(angleTangent);

        // Compute orbit as circle
        Vector2 inscribedCircle = Vector2.zero;
        if (Mathf.Sin(angleCenter) != 0)
        {
            inscribedCircle.x = Mathf.Abs(delta.x) / Mathf.Abs(Mathf.Sin(angleCenter));
            if (Mathf.Cos(angleCenter) == 0) inscribedCircle.y = inscribedCircle.x;
        }
        if (Mathf.Cos(angleCenter) != 0)
        {
            inscribedCircle.y = Mathf.Abs(delta.z) / Mathf.Abs(Mathf.Cos(angleCenter));
            if (Mathf.Sin(angleCenter) == 0) inscribedCircle.x = inscribedCircle.y;
        }
        

        result.size = inscribedCircle;
        return result;
    }
    public static Vector3 GetForceAt(Vector3 universePosition)
    {
        Vector3 force = Vector3.zero;
        foreach (CelestialRigidBody body in CelestialRigidBody.celestialBodies)
        {
            force += body.GetForceAt(universePosition);
        }

        return force;
    }
}
