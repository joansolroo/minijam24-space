using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class CollectibleHandler : MonoBehaviour
{
    [SerializeField] Text label;
    List<Collectible> collectibles = new List<Collectible>();

    int collectedCount = 0;

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
    }
}
