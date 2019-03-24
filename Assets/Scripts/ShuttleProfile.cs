using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuttleProfile : MonoBehaviour
{
    [SerializeField] public bool firstStage = true;
    [SerializeField] public int mainThrusters = 1;
    [SerializeField] public int sideThrusters = 1;
    [SerializeField] public int frontalThrusters = 0;
    [SerializeField] public int stabilizers = 0;
    [SerializeField] public int landingModule = 0;

    [SerializeField] public int maxFuelStage1 = 10;
    [SerializeField] public int maxFuel = 10;
    [SerializeField] public int maxHp = 10;
    [SerializeField] public int maxOxigen = 0;
    [SerializeField] public int maxFood = 0;
    [SerializeField] public int maxRange = 300;
}
