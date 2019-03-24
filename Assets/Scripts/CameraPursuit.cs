using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPursuit : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float transitionTime;
    [SerializeField] Camera camera;

    [SerializeField] [Range(40, 200)] float distance;
    [SerializeField] [Range(1, 50)] float zoomSpeed;
    [SerializeField] [Range(-1, 1)] float dragSpeed;
    [SerializeField] Vector2 rotation;

    Vector2 drag;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            drag = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            Vector2 newDrag = Input.mousePosition;

            rotation.y += -(newDrag.y - drag.y)/Camera.main.pixelHeight;
            rotation.y = Mathf.Clamp(rotation.y, -1, 0);
            rotation.x += (newDrag.x - drag.x) / Camera.main.pixelWidth;
            rotation.x = Mathf.Clamp(rotation.x, -1, 1);
            drag = newDrag;

        }
    }
    void LateUpdate()
    {
        this.transform.position = target.transform.position;
        this.transform.eulerAngles = new Vector3(90 + rotation.y * 90, 90 + rotation.x * 45, 0);
       
        
        distance -= Input.mouseScrollDelta.y* zoomSpeed;
        distance = Mathf.Min(200, Mathf.Max(40, distance));
        camera.transform.localPosition = new Vector3(0, 0, -distance / 2);
        camera.orthographicSize = Mathf.MoveTowards(camera.orthographicSize, distance, Time.deltaTime* 30*zoomSpeed);
    }
}
