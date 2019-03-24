using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Introduction : MonoBehaviour
{
    [SerializeField] float timeToHide = 15;
    [SerializeField] GameObject toHide;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoHide());
    }
    
    IEnumerator DoHide()
    {
        yield return new WaitForSeconds(timeToHide);
        toHide.SetActive(false);
    }
}
