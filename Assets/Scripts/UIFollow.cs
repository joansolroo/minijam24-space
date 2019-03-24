using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
[ExecuteInEditMode]
public class UIFollow : MonoBehaviour
{
    [Tooltip("World space object to follow")]
    public GameObject target;
    [Tooltip("World space camera that renders the target")]
    public Camera worldCamera;
    [Tooltip("Canvas set in Screen Space Camera mode")]
    public Canvas canvas;
    RectTransform rt;
    RectTransform parent;
    private void Start()
    {
        if(worldCamera==null)
            worldCamera = Camera.main;
        rt = GetComponent<RectTransform>();
        parent = (RectTransform)rt.parent;
    }
    private void Update()
    {
        UpdatePosition();
    }
    private void LateUpdate()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        var vp = worldCamera.WorldToViewportPoint(target.transform.position);
        var sp = canvas.worldCamera.ViewportToScreenPoint(vp);
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, sp, canvas.worldCamera, out worldPoint);
        rt.position = worldPoint;
    }
}