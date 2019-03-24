using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shuttle : MonoBehaviour
{
    [SerializeField] MisionControl missionControl;
    [SerializeField] GameObject shipModel;
    [SerializeField] Animation explosion;
    Rigidbody rb;

    [Header("Profile")]
    public ShuttleProfile profile;
    [SerializeField] float launchForce = 1;
    [SerializeField] float forceFoward = 1;
    [SerializeField] float forceSideways = 1;
    [SerializeField] float forceTorque = 1;

    [SerializeField] float forceScale = 1;
    [SerializeField] LayerMask layerMask;

    [Header("Status")]
    [SerializeField] public bool active = true;
    [SerializeField] public bool tripulated = false;
    [SerializeField] public bool launched = false;
    [SerializeField] public float fuelStage1;
    [SerializeField] public float fuel;
    [SerializeField] public float oxigen;
    [SerializeField] public float food;
    [SerializeField] public float hp;
    [SerializeField] public float ttl;
    [SerializeField] public float distanceToBase;
    [Header("paths")]
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

    public enum ShuttleAction
    {
        none, launch, left, right, forward, backwards, stabilize, deactivate
    }

    public bool PossibleAction(ShuttleAction action)
    {
        return true; //TODO use profile
    }
    ShuttleAction nextAction = ShuttleAction.none;
    public void RequestAction(ShuttleAction action)
    {
        if (PossibleAction(action))
        {
            nextAction = action;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (nextAction != ShuttleAction.none)
        {
            if (!launched)
            {
                if (nextAction == ShuttleAction.launch)
                {
                    StartCoroutine(DoLaunch());
                }
            }
            else
            {
                if (fuel > 0)
                {
                    if (nextAction == ShuttleAction.left)
                    {
                        rb.AddForce(this.transform.TransformVector(Vector3.left) * forceSideways);
                        rb.AddTorque(transform.up * -forceTorque);
                        action = true;
                    }
                    else if (nextAction == ShuttleAction.right)
                    {
                        rb.AddForce(this.transform.TransformVector(Vector3.right) * forceSideways);
                        rb.AddTorque(transform.up * forceTorque);
                        action = true;
                    }
                    else if (nextAction == ShuttleAction.forward)
                    {
                        rb.AddForce(this.transform.TransformVector(Vector3.forward) * forceFoward);
                        action = true;
                    }
                    else if (nextAction == ShuttleAction.backwards)
                    {
                        rb.AddForce(this.transform.TransformVector(Vector3.back) * forceFoward);
                        rb.AddTorque(transform.up * -forceTorque);
                        action = true;
                    }
                    else if (nextAction == ShuttleAction.stabilize)
                    {
                        rb.AddForce(this.transform.TransformVector(Vector3.back) * forceFoward);
                        transform.LookAt(transform.position + rb.velocity, Vector3.up);
                        action = true;
                    }
                }
            }
        }
        if (nextAction == ShuttleAction.deactivate)
        {
            if (hp > 0)
            {
                pastLine.startColor = pastLine.endColor = Color.gray;
                updatedLine.startColor = updatedLine.endColor = Color.gray;
                for (int c = 0; c < shipModel.transform.childCount; ++c)
                {
                    GameObject gChild = shipModel.transform.GetChild(c).gameObject;
                    Renderer renderer;
                    renderer = gChild.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.gray;
                    }
                    else
                    {
                        renderer = gChild.GetComponent<LineRenderer>();
                        if (renderer != null)
                        {
                            renderer.material.color = Color.gray;
                        }
                    }
                    
                }
                active = false;
            }
        }
        nextAction = ShuttleAction.none;
    }
    int frame = 0;
    private void Update()
    {
        if (hp > 0 && active)
        {
            if (action)
            {
                fuel--;
                action = false;
            }
            if (launched)
            {
                ttl = Mathf.Max(0, ttl - Time.deltaTime);
                distanceToBase = Vector3.Distance(missionControl.transform.position, transform.position);
                if (tripulated)
                {
                    food = Mathf.Max(0, food - Time.deltaTime);
                    oxigen = Mathf.Max(0, oxigen - Time.deltaTime);
                }
            }
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
    }

    IEnumerator DoLaunch()
    {
        transform.parent = null;
        launched = true;
        rb.isKinematic = false;
        rb.AddForce(this.transform.TransformVector(Vector3.forward) * launchForce);
        for(float t =0; t < 1f; t += Time.deltaTime)
        {
            fuelStage1 = (1-t)*profile.maxFuelStage1;
            yield return new WaitForEndOfFrame();
        }
        fuelStage1 *= 0;
        theoreticTrajectory = predictedTrajectory;
        theoryLine.positionCount = theoreticTrajectory.Count;
        theoryLine.SetPositions(theoreticTrajectory.ToArray());
    }

    [SerializeField] Color pathColorLost = new Color(255, 0, 0, 100);
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Shuttle collision (" + collision.gameObject.name + ")");
        hp = 0;
        explosion.Play();
        pastLine.startColor = pastLine.endColor = pathColorLost;
        updatedLine.startColor = updatedLine.endColor = pathColorLost;
        //updatedLine.gameObject.SetActive(false);
        shipModel.SetActive(false);
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
        if (theoreticTrajectory != null && theoreticTrajectory.Count > 0)
        {
            Vector3 point = theoreticTrajectory[0];
            for (int p = 1; p < theoreticTrajectory.Count; ++p)
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
