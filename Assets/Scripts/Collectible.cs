using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] CollectibleHandler handler;

    private void Start()
    {
        handler.Register(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        handler.CollectibleCollected();
        this.gameObject.SetActive(false);
    }
}
