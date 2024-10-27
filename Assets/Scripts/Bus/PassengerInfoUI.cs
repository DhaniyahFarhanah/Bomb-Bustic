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
    public List<GameObject> passengerIcons;
    private int inactivePassengers;
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

    public void InitPassengers(int passengerTotal)
    {
        inactivePassengers = passengerIcons.Count - passengerTotal;
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
    }

    public void DeliveredPassenger(int current)
    {
        passengerIcons[current - 1].GetComponent<PassengerIconStatus>().SetStatus(PassengerIconStatus.IconStatus.Delivered);
    }

    public void InjuredPassenger(int current)
    {
        passengerIcons[current - 1].GetComponent<PassengerIconStatus>().SetStatus(PassengerIconStatus.IconStatus.Injured);
    }

    public void LostPassenger(int current)
    {
        passengerIcons[current - 1].GetComponent<PassengerIconStatus>().SetStatus(PassengerIconStatus.IconStatus.Lost);
    }

    public void UpdatePassengerInfoText(int current, int delivered, int injured, int lost)
    {
        if (passengerInfoEnabled)
        {
            passengerText.text = $"Current: {current} | Delivered: {delivered} | Injured: {injured} | Lost: {lost}";
        }
    }

    public void UpdateCurrentIndicator(int current)
    {
        currentIndicator.transform.position = passengerIcons[current - 1].transform.position;
    }
}
