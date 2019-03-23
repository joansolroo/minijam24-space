using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPursuit : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float transitionTime;
   
    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = target.transform.position;

    }
}
