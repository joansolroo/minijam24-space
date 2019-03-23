using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRenderer : MonoBehaviour
{
    [SerializeField] [Range(1, 100)] int segmentation = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 pointInSurface(float u, float v)
    {
        float angleA = u * Mathf.PI * 2;
        float angleB = v * Mathf.PI * 2;
        Vector3 p = new Vector3(Mathf.Sin(angleA) * Mathf.Cos(angleB), Mathf.Sin(angleA) * Mathf.Sin(angleB), Mathf.Cos(angleA));
        p = Vector3.Scale(p, transform.lossyScale*0.5f)+this.transform.position;
        return p;
    }


    private void OnDrawGizmos()
    {
        float delta = 1f / segmentation;
        for(float u = 0; u <1; u += delta)
        {
            for (float v = 0; v < 1; v += delta)
            {
                Gizmos.DrawSphere(pointInSurface(u/*+ Random.Range(-delta/8,delta/8)*/, v/*+ Random.Range(-delta / 8, delta / 8)*/), delta);
            }
        }
    }
}
