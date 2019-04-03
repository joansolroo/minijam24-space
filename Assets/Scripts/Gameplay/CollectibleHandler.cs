using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CollectibleHandler : MonoBehaviour
{
    [SerializeField] Text label;
    List<Collectible> collectibles = new List<Collectible>();
    AudioSource audio;

    int collectedCount = 0;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    internal void Register(Collectible collectible)
    {
        collectibles.Add(collectible);
    }

    internal void CollectibleCollected()
    {
        ++collectedCount;
        if(collectedCount == 1)
        {
            label.gameObject.SetActive(true);
        }
        label.text = "Exploration: "+ collectedCount.ToString("00")+ "/" + collectibles.Count;
        audio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        audio.Play();
    }
}
