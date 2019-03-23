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

    private void Start()
    {
        if(worldCamera==null)
            worldCamera = Camera.main;
    }
    private void LateUpdate()
    {
        var rt = GetComponent<RectTransform>();
        RectTransform parent = (RectTransform)rt.parent;
        var vp = worldCamera.WorldToViewportPoint(target.transform.position);
        var sp = canvas.worldCamera.ViewportToScreenPoint(vp);
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, sp, canvas.worldCamera, out worldPoint);
        rt.position = worldPoint;
    }
}