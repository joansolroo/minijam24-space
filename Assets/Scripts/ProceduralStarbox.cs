using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralStarbox : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] GameObject starTemplate;
    // Start is called before the first frame update
    void Start()
    {
        Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Create() {

        float delta = 1f / 100;
        for (float u = 0; u < 1; u += delta)
        {
            for (float v = 0; v < 1; v += delta)
            {
                float angleA =( u +Random.Range(-delta/4,delta/4)) * Mathf.PI * 2;
                float angleB = (v + Random.Range(-delta / 4, delta / 4)) * Mathf.PI * 2;
                float r = Random.Range(2000, 5000);
                Vector3 p = new Vector3(Mathf.Sin(angleA) * Mathf.Cos(angleB), Mathf.Sin(angleA) * Mathf.Sin(angleB), Mathf.Cos(angleA))*r;
                p = transform.TransformPoint(p);
                GameObject go = GameObject.Instantiate<GameObject>(starTemplate);
                go.transform.position = p;
                go.transform.parent = this.transform;
                go.GetComponent<Renderer>().material = material;

            }
        }
    }
}
