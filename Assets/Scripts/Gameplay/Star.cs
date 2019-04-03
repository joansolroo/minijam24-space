using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public class Comp : IComparer<Collider>
    {
        Vector3 target;
        public Comp(Vector3 _target)
        {
            target = _target;
        }
        public int Compare(Collider x, Collider y)
        {
            // your custom compare code here.
            return Vector3.Distance(x.transform.position, target).CompareTo(Vector3.Distance(y.transform.position, target));
        }
    }
    Collider[] neighbors;

    private void Start()
    {
        ComputeNeighbors();
    }
    void ComputeNeighbors (){
       neighbors = Physics.OverlapSphere(this.transform.position, 10);
        Array.Sort(neighbors, new Comp(this.transform.position));
    }
    private void OnDrawGizmos()
    {

        
        Gizmos.color = new Color(0, 1, 1, 0.05f);

        for (int n = 0; n < 4 && n < neighbors.Length; ++n)
        {
            Gizmos.DrawLine(this.transform.position, neighbors[n].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
