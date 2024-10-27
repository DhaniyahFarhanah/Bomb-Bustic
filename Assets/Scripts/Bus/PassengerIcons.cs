using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassengerIcons : MonoBehaviour
{
    public bool passengerInfoEnabled;
    [SerializeField] private GameObject passengerTextObject;
    [SerializeField] private TextMeshProUGUI passengerText;
    [SerializeField] private RectTransform passengerBackground;
    public List<GameObject> passengerIcons;
    private int inactivePassengers;
    private int lastPassenger = 0;
    private float extraPassengerRowHeight = 50f;
    private float passengerInfoHeight = 80f;

    public void InitPassengers(int passengerTotal)
    {
        inactivePassengers = passengerIcons.Count - passengerTotal;
        for (int i = 0; i < inactivePassengers; ++i)
        {
            passengerIcons[i].SetActive(false);
        }
        lastPassenger += inactivePassengers;

        Vector2 size = passengerBackground.sizeDelta;
        if (passengerTotal <= 20)
        {
            size.y -= extraPassengerRowHeight;
        }
        if (!passengerInfoEnabled)
        {
            size.y -= passengerInfoHeight;
        }
        passengerBackground.sizeDelta = size;

        passengerTextObject.SetActive(passengerInfoEnabled);
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

    public void UpdatePassengerInfoText(int current, int delivered, int injured, int lost)
    {
        if (passengerInfoEnabled)
        {
            passengerText.text = $"Current: {current} | Delivered: {delivered} | Injured: {injured} | Lost: {lost}";
        }
    }
}
