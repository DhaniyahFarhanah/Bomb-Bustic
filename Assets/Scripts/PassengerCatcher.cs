using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassengerCatcher : MonoBehaviour
{
    [SerializeField] private int catchCapacity;
    [SerializeField] private TextMeshPro catchText;
    [SerializeField] private GameObject nearArea;

    private int catchCurrent = 0;
    private Camera mainCamera;

    private void Start()
    {
        // Get reference to the main camera
        mainCamera = Camera.main;
        UpdateCatchText();
    }

    private void Update()
    {
        BillboardText();
    }

    private void UpdateCatchText()
    {
        catchText.text = catchCurrent + " / " + catchCapacity;
        if (catchCurrent >= catchCapacity)
        {
            catchText.text += "\nMax Capacity";
        }
    }

    private void BillboardText()
    {
        // Make the text always face the camera
        catchText.transform.LookAt(mainCamera.transform);
        catchText.transform.Rotate(0, 180, 0); // Rotate 180 degrees because LookAt might point the text away
    }

    public void CaughtPassenger()
    {
        ++catchCurrent;
        UpdateCatchText();

        if (catchCurrent >= catchCapacity)
        {
            GetComponent<BoxCollider>().isTrigger = false;
            nearArea.SetActive(false);
            FindAnyObjectByType<BusPassengers>().EnablePassengerEjection(false);
        }
    }

    public bool CheckCapacity()
    {
        return catchCurrent < catchCapacity;
    }
}
