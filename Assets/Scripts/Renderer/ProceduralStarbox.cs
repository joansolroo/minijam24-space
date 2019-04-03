using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralStarbox : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] GameObject starTemplate;

    [SerializeField] float distanceMin =1000;
    [SerializeField] float distanceMax = 2000;
    [SerializeField] int countSqrt = 100;

    [SerializeField] Gradient color;
    [SerializeField] AnimationCurve size;
    [SerializeField] Vector3 volumeSize;
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

        float delta = 1f / countSqrt;
        for (float u = 0; u < 1; u += delta)
        {
            for (float v = 0; v < 1; v += delta)
            {
                float u2 = u+Random.Range(-delta / 2, delta / 2);
                float v2 = v+Random.Range(-delta / 2, delta / 2);
                float r = Random.Range(distanceMin, distanceMax);
                Vector3 p = Sphere.Evaluate(u2, v2)*r;
                p = transform.TransformPoint(p);
                p.Scale(volumeSize);
                GameObject go = GameObject.Instantiate<GameObject>(starTemplate);
                go.transform.localPosition = p;
                go.transform.parent = this.transform;
                go.transform.localScale = Vector3.one * size.Evaluate(Random.Range(0f,1f));
                go.GetComponent<Renderer>().material.color =color.Evaluate(Random.Range(0f,1f));

            }
        }
    }
}
