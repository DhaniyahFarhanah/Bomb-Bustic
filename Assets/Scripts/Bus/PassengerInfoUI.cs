using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassengerInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject ObjectiveUI;
    [SerializeField] private TextMeshProUGUI passengerCount;

    private void Awake()
    {
    }

    private void Start()
    {
        StartCoroutine(DeactivateObjectiveText());
    }

    private IEnumerator DeactivateObjectiveText()
    {
        yield return new WaitForSeconds(5f);
        ObjectiveUI.SetActive(false);
    }

    public void SetPassengersCountText(int num)
    {
        passengerCount.text = num.ToString();
    }
}
