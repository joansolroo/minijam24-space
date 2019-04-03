using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EllipseRenderer : MonoBehaviour
{
    [SerializeField] public Vector2 shape;
    [SerializeField] float shapeNoise;
    
    [SerializeField] [Range(4, 50)] int segments = 10;

    LineRenderer renderer;
    Vector3[] points;

    private void OnValidate()
    {
        Start();
    }
    private void Start()
    {
        if (renderer == null)
        {
            renderer = GetComponent<LineRenderer>();
        }
        Recompute();
    }
    // Update is called once per frame
    void Update()
    {

    }
    void Recompute()

    {
        shape += new Vector2(Random.Range(-shapeNoise, shapeNoise), Random.Range(-shapeNoise, shapeNoise));

        if (points == null || renderer.positionCount != points.Length)
        {
            points = new Vector3[segments];
            renderer.positionCount = segments;
        }
        for (int s = 0; s < segments; ++s)
        {
            points[s] = Ellipse.Evaluate(Vector3.zero,shape,((float)s) / segments); //TODO:improve
        }

        renderer.SetPositions(points);
        renderer.loop = true;
    }

    public Vector3 GetPointInUniverse(float t)
    {
        return transform.TransformPoint(Ellipse.Evaluate(Vector3.zero, shape, t));
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 point in points)
        {
            Gizmos.DrawSphere(transform.TransformPoint(point), 0.1f);
        }
    }
}
