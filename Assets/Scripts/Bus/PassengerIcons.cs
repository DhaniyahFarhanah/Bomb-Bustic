using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassengerIcons : MonoBehaviour
{
    public List<GameObject> passengerIcons;
    private int inactivePassengers;
    private int lastPassenger = 0;

    public void InitPassengers(int passengerTotal)
    {
        inactivePassengers = passengerIcons.Count - passengerTotal;
        for (int i = 0; i < inactivePassengers; ++i)
        {
            passengerIcons[i].SetActive(false);
        }
        lastPassenger += inactivePassengers;
    }

    public void DeliveredPassenger()
    {
        passengerIcons[lastPassenger].GetComponent<Image>().color = Color.green;
        ++lastPassenger;
    }

    public void InjuredPassenger()
    {
        passengerIcons[lastPassenger].GetComponent<Image>().color = Color.yellow;
        ++lastPassenger;
    }

    public void LostPassenger()
    {
        passengerIcons[lastPassenger].GetComponent<Image>().color = Color.red;
        ++lastPassenger;
    }
}
