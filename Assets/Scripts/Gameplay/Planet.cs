using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public int seed;
    public Vector2 min;
    public Vector2 max;

    List<Vector3> cities = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        float cityRange = 0.2f;
        float buildingRange = 0.05f;
        int citySize = 30;
        Random.InitState(seed);

        for (float u = min.x; u <= max.x; u += 0.25f)
        {
            for (float v = min.y; v < max.y; v += 0.25f)
            {
                Vector2 city = new Vector2(u + Random.Range(-cityRange, cityRange), v + Random.Range(-cityRange, cityRange));
                GameObject cityGo = new GameObject();
                cityGo.name = "City";
                cityGo.transform.parent = this.transform;
                cityGo.transform.localPosition = Sphere.Evaluate(city.x, city.y);
                cityGo.transform.LookAt(this.transform.position, this.transform.up);

                cities.Add(cityGo.transform.localPosition);

                int size = Random.Range(citySize / 2, citySize);
                for (int i = 0; i < citySize; ++i)
                {

                    Vector2 building = new Vector2(city.x + Random.Range(-buildingRange, buildingRange), city.y + Random.Range(-buildingRange, buildingRange));
                    Vector3 pos = Sphere.Evaluate(building.x, building.y);
                    GameObject buildingGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    buildingGo.GetComponent<Collider>().enabled = false; //TODO remove
                    buildingGo.name = "building";
                    buildingGo.transform.parent = this.transform;
                    buildingGo.transform.localPosition = pos;
                    buildingGo.transform.LookAt(this.transform.position, this.transform.up);
                    buildingGo.transform.localScale = new Vector3(Random.Range(0.5f, 3.9f), Random.Range(0.5f, 2.9f), Random.Range(0.9f, 10.9f)) * 0.01f * (Vector2.Distance(city, building) / buildingRange);
                    buildingGo.transform.Translate(-buildingGo.transform.localScale * 0.25f, Space.Self);

                    buildingGo.transform.parent = cityGo.transform;
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        /*
        Random.InitState(seed);
        for (float u = min.x; u <= max.x; u += 0.1f)
        {
            for (float v = min.y; v < max.y; v += 0.1f)
            {
                Gizmos.color = new Color(u, v, 0);

                Gizmos.DrawSphere(transform.TransformPoint(Sphere.Evaluate(u + Random.Range(-0.02f, 0.02f), v + Random.Range(-0.02f, 0.02f))), 0.025f);
            }
        }*/
        foreach(Vector3 city in cities)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.TransformPoint(city),0.1f);
        }
    }
}
