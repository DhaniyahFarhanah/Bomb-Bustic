using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerLanding : MonoBehaviour
{
    private bool collided = false;

    private void OnTriggerEnter(Collider other)  // Change to Collider for triggers
    {
        if (other.gameObject.CompareTag("DropOff"))  // The DropOff object should have the tag "DropOff"
        {
            FindAnyObjectByType<BusPassengers>().DeliveredPassenger();
            StopAllCoroutines();
        }
    }
    private void OnCollisionEnter(Collision other)  // Change to Collider for triggers
    {
        if (!collided)
        {
            collided = true;
            StartCoroutine(StartInjuredCountdown());
        }
    }

    private IEnumerator StartInjuredCountdown()
    {
        yield return new WaitForSeconds(1f);
        FindAnyObjectByType<BusPassengers>().InjuredPassenger();
    }
}
