using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerLanding : MonoBehaviour
{
    private enum PassengerState
    {
        Delivered,
        Injured,
        Lost,
    }
    private PassengerState passengerState;
    private bool collided = false;

    private void OnTriggerEnter(Collider other)  // Change to Collider for triggers
    {
        if (other.gameObject.CompareTag("NearDropOff"))  // The DropOff object should have the tag "DropOff"
        {
            passengerState = PassengerState.Injured;
        }
        if (other.gameObject.CompareTag("DropOff"))  // The DropOff object should have the tag "DropOff"
        {
            StopAllCoroutines();
            passengerState = PassengerState.Delivered;
            FindAnyObjectByType<BusPassengers>().DeliveredPassenger();
        }
    }
    private void OnCollisionEnter(Collision other)  // Change to Collider for triggers
    {
        if (!collided)
        {
            collided = true;
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(1f);
        if (passengerState == PassengerState.Injured)
        {
            FindAnyObjectByType<BusPassengers>().InjuredPassenger();
        }
        else
        {
            FindAnyObjectByType<BusPassengers>().LostPassenger();
        }
    }
}
