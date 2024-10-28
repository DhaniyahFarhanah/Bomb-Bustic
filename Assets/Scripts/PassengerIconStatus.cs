using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassengerIconStatus : MonoBehaviour
{   
    public enum IconStatus
    {
        Healthy,
        Delivered,
        DeliveredInjured,
        Injured,
        Lost,
    }
    public IconStatus currentStatus = IconStatus.Healthy;

    private void Start()
    {
        SetStatus(IconStatus.Healthy);
    }

    public void SetStatus(IconStatus newStatus)
    {
        switch (newStatus)
        {
            case IconStatus.Healthy:
                GetComponent<Image>().color = Color.white;
                break;
            case IconStatus.Delivered:
                if (currentStatus == IconStatus.Injured)
                {
                    GetComponent<Image>().color = Color.magenta;
                    currentStatus = IconStatus.DeliveredInjured;
                }
                else if (currentStatus == IconStatus.Healthy)
                {
                    GetComponent<Image>().color = Color.green;
                    currentStatus = IconStatus.Delivered;
                }
                break;
            case IconStatus.Injured:
                if (currentStatus != IconStatus.Lost)
                {
                    GetComponent<Image>().color = Color.yellow;
                    currentStatus = IconStatus.Injured;
                }
                break;
            case IconStatus.Lost:
                GetComponent<Image>().color = Color.red;
                currentStatus = IconStatus.Lost;
                break;
        }
    }
}
