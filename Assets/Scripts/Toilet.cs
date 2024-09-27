using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeVehicleController;

public class Toilet : MonoBehaviour
{
    public float reliefAmount = 45f;

    public void GoToilet(Vehicle bus)
    {
        bus.GetComponent<PoopMeter>().ReducePoop(reliefAmount);
    }
}
