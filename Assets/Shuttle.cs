using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shuttle : MonoBehaviour
{
   
    Rigidbody rb;
    [SerializeField] bool launched = false;
    [SerializeField] float launchForce = 1;
    [SerializeField] float forceFoward = 1;
    [SerializeField] float forceSideways = 1;
    [SerializeField] float forceTorque = 1;

    [SerializeField] float forceScale = 1;
    [SerializeField] LayerMask layerMask;

    [SerializeField] Transform missionControl;
    [SerializeField] float fuel;
    [SerializeField] float food;
    [SerializeField] float ttl;
    [SerializeField] float distanceToBase;

    [SerializeField] LineRenderer theoryLine;
    [SerializeField] LineRenderer updatedLine;
    [SerializeField] LineRenderer pastLine;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = !launched;
    }
    List<Vector3> theoreticTrajectory;
    List<Vector3> predictedTrajectory;
    List<Vector3> currentTrajectory = new List<Vector3>();
    bool action = false;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!launched)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool expectedCollision;
                theoreticTrajectory = ComputeTrajectory(out expectedCollision);
                theoryLine.positionCount = theoreticTrajectory.Count;
                theoryLine.SetPositions(theoreticTrajectory.ToArray());

                transform.parent = null; 
                launched = true;
                rb.isKinematic = false;
                rb.AddForce(this.transform.TransformVector(Vector3.forward) * launchForce);
                
            }
        }
        else {
            if (fuel > 0)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rb.AddForce(this.transform.TransformVector(Vector3.left) * forceSideways);
                    rb.AddTorque(transform.up * -forceTorque);
                    action = true;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    rb.AddForce(this.transform.TransformVector(Vector3.right) * forceSideways);
                    rb.AddTorque(transform.up * forceTorque);
                    action = true;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    rb.AddForce(this.transform.TransformVector(Vector3.forward) * forceFoward);
                    action = true;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    rb.AddForce(this.transform.TransformVector(Vector3.back) * forceFoward);
                    rb.AddTorque(transform.up * -forceTorque);
                    action = true;
                }
            }
        }
        
    }
    int frame = 0;
    private void Update()
    {
        if (action)
        {
            fuel -= Time.deltaTime;
            action = false;
        }
        ttl -= Time.deltaTime;
        distanceToBase = Vector3.Distance(missionControl.position, transform.position);
        ++frame;
        if (frame % 10 == 0)
        {
            frame = 0;
            bool expectedCollision;
            predictedTrajectory = ComputeTrajectory(out expectedCollision);
            if (expectedCollision)
            {
                updatedLine.endColor = Color.red;
            }
            else
            {
                updatedLine.endColor = Color.green;
            }
            updatedLine.positionCount = predictedTrajectory.Count;
            updatedLine.SetPositions(predictedTrajectory.ToArray());
            if (launched)
            {
                int currentIdx = currentTrajectory.Count;
                currentTrajectory.Add(transform.position);
                pastLine.positionCount = currentIdx + 1;
                pastLine.SetPosition(currentIdx, currentTrajectory[currentIdx]);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("boom!");
    }

    List<Vector3> ComputeTrajectory(out bool expectedCollision)
    {
        expectedCollision = false;
        float duration = 10;
        float step = Time.deltaTime * 5; //MAGIC NUMBER :(
        int collisionTest = (int)(10 / Time.timeScale);
        Vector3 pos = transform.position;
        Vector3 velocity = launched ? rb.velocity : transform.TransformVector(Vector3.forward) * step * launchForce / 5; //MAGIC NUMBER :(

       // Gizmos.DrawLine(pos, pos + velocity);

        List<Vector3> points = new List<Vector3>();
        points.Add(pos);
        bool collision = false;
        int i = 0;
        for (float t = 0; !collision && t < duration; t += step)
        {
            
            Vector3 newPos = pos + velocity * step;
            Vector3 forceAtnewPos = CelestialRigidBody.GetForceAt(newPos) * forceScale;
           // Gizmos.DrawSphere(newPos, 0.1f);
            //Gizmos.DrawLine(pos, newPos);
            pos = newPos;
            velocity = velocity + forceAtnewPos;
            
            //crash prediction
            i++;
            if (i % collisionTest == 0)
            {
                collision = Physics.OverlapSphere(newPos, safeDiameter, layerMask).Length > 0;
                
               /* if (collision)
                {
                    Gizmos.DrawWireSphere(newPos, safeDiameter);
                }*/
            }
            points.Add(newPos);
        }
        expectedCollision = collision;
        return points;
    }
    [SerializeField] float safeDiameter = 1;
    private void OnDrawGizmos()
    {
        float step = Time.fixedDeltaTime * 5; //MAGIC NUMBER :(
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Vector3 velocity = launched ? rb.velocity : transform.TransformVector(Vector3.forward) * step * launchForce / 2; //MAGIC NUMBER :(

        // Gizmos.DrawLine(pos, pos + velocity);
        Gizmos.color = Color.green;
        if (theoreticTrajectory != null && theoreticTrajectory.Count>0)
        {
            Vector3 point = theoreticTrajectory[0];
            for(int p = 1; p < theoreticTrajectory.Count; ++p)
            {
                Vector3 point2 = theoreticTrajectory[p];
                Gizmos.DrawLine(point, point2);
                point = point2;
            }
        }
        Gizmos.color = Color.gray;
        if (predictedTrajectory != null && predictedTrajectory.Count > 0)
        {
            Vector3 point = predictedTrajectory[0];
            for (int p = 1; p < predictedTrajectory.Count; ++p)
            {
                Vector3 point2 = predictedTrajectory[p];
                Gizmos.DrawLine(point, point2);
                point = point2;
            }
        }
        Gizmos.color = Color.white;
        if (currentTrajectory != null && currentTrajectory.Count > 0)
        {
            Vector3 point = currentTrajectory[0];
            for (int p = 1; p < currentTrajectory.Count; ++p)
            {
                Vector3 point2 = currentTrajectory[p];
                Gizmos.DrawLine(point, point2);
                point = point2;
            }
        }
    }
}
