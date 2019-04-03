using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 100)] float timeScale=1;
    [SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        time += timeScale;
    }
}
