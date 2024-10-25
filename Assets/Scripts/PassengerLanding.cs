using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerLanding : MonoBehaviour
{
    private enum PassengerState
    {
        Starting,
        Delivered,
        Injured,
        Lost,
    }
    private PassengerState passengerState = PassengerState.Starting;
    private bool collided = false;
    private Rigidbody passengerRb;

    private void Start()
    {
        passengerRb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (passengerState == PassengerState.Delivered) return; // Avoid double counting

        if (other.gameObject.CompareTag("NearDropOff"))
        {
            passengerState = PassengerState.Injured;
        }
        else if (other.gameObject.CompareTag("DropOff"))
        {
            // Mark as delivered and prevent further physics interactions
            passengerState = PassengerState.Delivered;
            StopAllCoroutines();
            StartCoroutine(SlowAndFreezePassenger());
            FindAnyObjectByType<BusPassengers>().DeliveredPassenger();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!collided && passengerState != PassengerState.Delivered)
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
        else if (passengerState == PassengerState.Starting) // Ensure it's not delivered
        {
            FindAnyObjectByType<BusPassengers>().LostPassenger();
        }
    }

    private IEnumerator SlowAndFreezePassenger()
    {
        // Gradually reduce velocity before making the Rigidbody kinematic
        while (passengerRb.velocity.magnitude > 2f)
        {
            // Reduce the velocity gradually
            passengerRb.velocity = Vector3.Lerp(passengerRb.velocity, Vector3.zero, Time.deltaTime * 10f);
            yield return null;
        }
    }
}
