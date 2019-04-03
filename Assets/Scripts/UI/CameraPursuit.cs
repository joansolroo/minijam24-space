using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraPursuit : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] [Range(1, 120)] float transitionTime;
    [SerializeField] Camera camera;

    [SerializeField] [Range(1, 1000)] float distance;
    [SerializeField] [Range(1, 50)] float zoomSpeed;
    [SerializeField] [Range(-1, 1)] float dragSpeed;
    [SerializeField] Vector2 rotation;

    Vector2 drag;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (/*!EventSystem.current.IsPointerOverGameObject()
             &&*/ Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100000))
            {
                target = hit.transform;
            }
            else
            {
                target = null;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            drag = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            Vector2 newDrag = Input.mousePosition;

            rotation.y += -(newDrag.y - drag.y) / Camera.main.pixelHeight;
            rotation.y = Mathf.Clamp(rotation.y, -1, 0);
            rotation.x += (newDrag.x - drag.x) / Camera.main.pixelWidth;
            //rotation.x = Mathf.Clamp(rotation.x, -1, 1);
            drag = newDrag;

        }
    }

    void LateUpdate()
    {
        Vector3 targetPosition;

        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            targetPosition = Vector3.zero;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, Vector3.Distance(this.transform.position, targetPosition) * (121 - transitionTime) * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(90 + rotation.y * 90, 90 + rotation.x * 180, 0);


        distance -= Input.mouseScrollDelta.y * zoomSpeed;
        distance = Mathf.Min(1000, Mathf.Max(1, distance));
        camera.transform.localPosition = new Vector3(0, 0, -100/*-distance / 2*/);
        camera.orthographicSize = Mathf.MoveTowards(camera.orthographicSize, distance, Time.deltaTime * 30 * zoomSpeed);
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.DrawWireSphere(target.transform.position, 0.25f);
        }
    }
}
