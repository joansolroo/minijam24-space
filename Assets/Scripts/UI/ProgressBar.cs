using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class ProgressBar : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] string name;
    [SerializeField] Color colorBase;
    [SerializeField] Color colorEmpty;
    [SerializeField] [Range(0,100)] public float value;
    [Header("UI elements")]
    [SerializeField] Text text;
    [SerializeField] Image[] cells;
    [SerializeField] Outline[] outlines;

    private void Start()
    {
        text.text = name;
        outlines = new Outline[cells.Length];
        for (int c = 0; c < cells.Length; ++c)
        {
            outlines[c] = cells[c].GetComponent<Outline>();
        }
        UpdateVisuals();
    }
    private void OnValidate()
    {
        Start();
    }
    public void SetValue(float newValue)
    {
        value = newValue;
        UpdateVisuals();
    }

    public int min;
    void UpdateVisuals()
    {
        min = (int)(value / 100 * (cells.Length));
        for (int idx = 0; idx < cells.Length; ++idx)
        {
            if (value > 0 && idx <= min)
            {
                cells[idx].color = colorBase;
            }
            else
            {
                cells[idx].color = colorEmpty;
            }
            outlines[idx].effectColor = colorBase;
        }
    }
}

