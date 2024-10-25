using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BusPassengers : MonoBehaviour
{
    [Header("Passengers Settings")]
    [SerializeField] private GameObject passengerPrefab;
    [Range(1, 40)]
    [SerializeField] private int passengerTotal = 20;
    private int passengerCurrent;
    private int passengerDelivered = 0;
    private int passengerInjured = 0;
    private int passengerLost = 0;

    [Header("Exit Settings")]
    [SerializeField] private Vector3 exitOffset;
    [SerializeField] private float passengerExitForce = 1000f;
    [SerializeField] private float angleUp = 30f;

    [Header("UI")]
    [SerializeField] private PassengerIcons passengerIcons;
    [SerializeField] private TextMeshProUGUI passengerText;

    // Start is called before the first frame update
    void Start()
    {
        passengerCurrent = passengerTotal;
        passengerIcons.InitPassengers(passengerTotal);
        UpdatePassengerText();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void UpdatePassengerText()
    {
        passengerText.text = "Current: " + passengerCurrent + " | Delivered: " + passengerDelivered + " | Injured: " + passengerInjured + " | Lost: " + passengerLost;
    }

    private void HandleInput()
    {
        if (passengerCurrent > 0)
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse button
            {
                ShootPassenger(-transform.right, true); // Shoot to the left based on bus orientation
            }
            else if (Input.GetMouseButtonDown(1)) // Right mouse button
            {
                ShootPassenger(transform.right, false); // Shoot to the right based on bus orientation
            }
        }
    }

    private void ShootPassenger(Vector3 direction, bool isLeft)
    {
        // Calculate the upward direction using the angleUp
        Vector3 directionWithAngle;
        if (isLeft)
        {
            // When shooting to the left, angle up by rotating downward slightly
            directionWithAngle = Quaternion.AngleAxis(-angleUp, transform.forward) * direction;
        }
        else
        {
            // When shooting to the right, angle up by rotating upward
            directionWithAngle = Quaternion.AngleAxis(angleUp, transform.forward) * direction;
        }

        // Calculate the position based on the exit offset, using the bus's local orientation
        Vector3 exitPosition = transform.TransformPoint(exitOffset);

        // Instantiate the passenger at the calculated exit position
        GameObject passenger = Instantiate(passengerPrefab, exitPosition, Quaternion.identity);

        // Get the Rigidbody component
        Rigidbody rb = passenger.GetComponent<Rigidbody>();

        // Apply a force in the specified direction with upward tilt
        if (rb != null)
        {
            rb.AddForce(directionWithAngle * passengerExitForce);
        }

        // Decrease the number of current passengers
        passengerCurrent--;
        UpdatePassengerText();
    }

    public void DeliveredPassenger()
    {
        ++passengerDelivered;
        passengerIcons.DeliveredPassenger();
    }

    public void InjuredPassenger()
    {
        ++passengerInjured;
        passengerIcons.InjuredPassenger();
    }

    public void LostPassenger()
    {
        ++passengerLost;
        passengerIcons.LostPassenger();
    }
}
