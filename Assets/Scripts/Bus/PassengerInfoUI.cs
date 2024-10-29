using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassengerInfoUI : MonoBehaviour
{
    public bool passengerInfoEnabled;
    [SerializeField] private GameObject ObjectiveObject;
    [SerializeField] private GameObject passengerTextObject;
    [SerializeField] private GameObject currentIndicator;
    [SerializeField] private TextMeshProUGUI passengerText;
    [SerializeField] private RectTransform passengerBackground;
    [SerializeField] private List<GameObject> passengerIcons;
    private float extraPassengerRowHeight = 50f;
    private float passengerInfoHeight = 80f;

    private void Awake()
    {
        passengerIcons.Reverse();
    }

    private void Start()
    {
        StartCoroutine(DeactivateObjectiveText());
    }

    private IEnumerator DeactivateObjectiveText()
    {
        yield return new WaitForSeconds(5f);
        ObjectiveObject.SetActive(false);
    }

    public List<PassengerStatus> InitPassengers(int passengerTotal)
    {
        int inactivePassengers = passengerIcons.Count - passengerTotal;
        for (int i = 0; i < inactivePassengers; ++i)
        {
            passengerIcons[i + passengerTotal].SetActive(false);
        }
        currentIndicator.transform.position = passengerIcons[passengerTotal - 1].transform.position;

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

        List<PassengerStatus> temp = new List<PassengerStatus>();
        for(int i = 0; i < passengerTotal; ++i)
        {
            passengerIcons[i].GetComponent<PassengerStatus>().passengerID = i;
            temp.Add(passengerIcons[i].GetComponent<PassengerStatus>());
        }
        return temp;
    }

    public void UpdateDeliveredPassengerUI(int index)
    {
        passengerIcons[index].GetComponent<PassengerStatus>().SetStatus(PassengerStatus.Status.Delivered);
    }

    public void UpdateInjuredPassengerUI(int index)
    {
        passengerIcons[index].GetComponent<PassengerStatus>().SetStatus(PassengerStatus.Status.Injured);
    }

    public void UpdateLostPassengerUI(int index)
    {
        passengerIcons[index].GetComponent<PassengerStatus>().SetStatus(PassengerStatus.Status.Lost);
    }

    public void UpdatePassengerInfoTextUI(int current, int delivered, int injured, int lost)
    {
        if (passengerInfoEnabled)
        {
            passengerText.text = $"Current: {current} | Delivered: {delivered} | Injured: {injured} | Lost: {lost}";
        }
    }

    public void UpdateCurrentIndicator(int index)
    {
        if (index >= 0)
            currentIndicator.transform.position = passengerIcons[index].transform.position;
        else
            currentIndicator.SetActive(false);
    }
}
