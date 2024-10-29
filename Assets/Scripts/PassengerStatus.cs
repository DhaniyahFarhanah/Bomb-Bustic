using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassengerStatus : MonoBehaviour
{   
    public enum Status
    {
        Healthy,
        Delivered,
        DeliveredInjured,
        Injured,
        Lost,
    }
    private Status currentStatus = Status.Healthy;
    public int passengerID;

    private void Start()
    {
        SetStatus(Status.Healthy);
    }

    public bool SetStatus(Status newStatus)
    {
        switch (newStatus)
        {
            case Status.Healthy:
                if (currentStatus == Status.Healthy)
                {
                    GetComponent<Image>().color = Color.white;
                    currentStatus = Status.Healthy;
                    return true;
                }
                break;

            case Status.Delivered:
                if (currentStatus == Status.Healthy)
                {
                    GetComponent<Image>().color = Color.green;
                    currentStatus = Status.Delivered;
                    return true;
                }
                else if (currentStatus == Status.Injured)
                {
                    GetComponent<Image>().color = Color.yellow;
                    currentStatus = Status.DeliveredInjured;
                    return true;
                }
                break;

            case Status.Injured:
                if (currentStatus == Status.Healthy || currentStatus == Status.Injured)
                {
                    GetComponent<Image>().color = Color.yellow;
                    currentStatus = Status.Injured;
                    return true;
                }
                break;

            case Status.Lost:
                if (currentStatus != Status.Delivered || currentStatus != Status.DeliveredInjured)
                {
                    GetComponent<Image>().color = Color.red;
                    currentStatus = Status.Lost;
                    return true;
                }
                break;
        }
        return false;
    }

    public Status GetStatus()
    {
        return currentStatus;
    }
}
