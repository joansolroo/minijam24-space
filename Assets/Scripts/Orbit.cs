using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] EllipseRenderer orbitRenderer;

    [SerializeField] [Range(1,1000)]float period;

    [SerializeField] [Range(0,1)]float t;
    [SerializeField] bool randomStart = false;

    private void Start()
    {
        if (randomStart)
        {
            t = Random.Range(0f, 1f);
        }   
    }
    private void OnValidate()
    {
        Start();
        Update();
        
       // orbitRenderer.transform.position = parent.transform.position + offset;
    }
    private void Update()
    {
        t += Time.deltaTime / period;
        t = t % 1;
        Vector3 p = orbitRenderer.GetPointInUniverse(t);
        this.transform.position = p;
       // this.transform.localEulerAngles = new Vector3(0, Time.time % 1 * 360, 0);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, parent.transform.position);
        Gizmos.DrawLine(parent.transform.position, parent.transform.position);
    }*/
}
