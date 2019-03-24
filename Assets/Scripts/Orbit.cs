using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Orbit : MonoBehaviour
{
    [SerializeField] EllipseRenderer orbitRenderer;

    [SerializeField] [Range(1,1000)]float period;

    [SerializeField] int firstDay;
    [SerializeField] float daysYear;
    [SerializeField] float day;
    [SerializeField] [Range(0,1)]float t;
    [SerializeField] bool randomStart = false;

    [SerializeField] Text calendar;
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
        day = firstDay+t* daysYear;
        if (calendar != null)
        {
            calendar.text = "DATE: SOL " + (int)day;
        }
        Vector3 p = orbitRenderer.GetPointInUniverse(t % 1);
        this.transform.position = p;
       // this.transform.localEulerAngles = new Vector3(0, Time.time % 1 * 360, 0);

    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, parent.transform.position);
        Gizmos.DrawLine(parent.transform.position, parent.transform.position);
    }*/
}
