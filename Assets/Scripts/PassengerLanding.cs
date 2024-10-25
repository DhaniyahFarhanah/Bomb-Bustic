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
    private Rigidbody[] passengerRigidbodies;
    private bool done = false;

    private void Start()
    {
        // Get all Rigidbody components in the ragdoll (the passenger object and its children)
        passengerRigidbodies = GetComponentsInChildren<Rigidbody>();
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
            if (!done)
            {
                done = true;
                FindAnyObjectByType<BusPassengers>().DeliveredPassenger();
            }
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
            if (!done)
            {
                done = true;
                FindAnyObjectByType<BusPassengers>().InjuredPassenger();
            }
        }
        else if (passengerState == PassengerState.Starting) // Ensure it's not delivered
        {
            if (!done)
            {
                done = true;
                FindAnyObjectByType<BusPassengers>().LostPassenger();
            }
        }
    }

    private IEnumerator SlowAndFreezePassenger()
    {
        // Slow down all rigidbodies in the ragdoll
        bool stillMoving = true;

        while (stillMoving)
        {
            stillMoving = false; // Assume no rigidbody is moving at the start of the loop

            foreach (Rigidbody rb in passengerRigidbodies)
            {
                if (rb.velocity.magnitude > 2f)
                {
                    stillMoving = true; // At least one rigidbody is still moving

                    // Gradually reduce the velocity of each Rigidbody
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 10f);
                    rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * 10f);
                }
            }

            yield return null;
        }

        // Once all bodies are sufficiently slow, freeze the ragdoll
        foreach (Rigidbody rb in passengerRigidbodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Make them kinematic to prevent further physics interactions
        }
    }
}