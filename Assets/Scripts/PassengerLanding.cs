using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerLanding : MonoBehaviour
{
    private Rigidbody[] passengerRigidbodies;
    private bool collided = false;
    private bool caught = false;

    private void Start()
    {
        // Get all Rigidbody components in the ragdoll (the passenger object and its children)
        passengerRigidbodies = GetComponentsInChildren<Rigidbody>();
        StartCoroutine(LostAfterDelay(5f));
    }

    private void OnTriggerEnter(Collider other)
    {         
        if (other.gameObject.CompareTag("DropOff"))
        {
            if (other.gameObject.GetComponent<PassengerCatcher>().HasVacancy() && !caught)
            {
                caught = true;
                StopAllCoroutines();
                StartCoroutine(SlowAndFreezePassenger());
                FindAnyObjectByType<BusPassengers>().DeliveredPassenger();
                other.gameObject.GetComponent<PassengerCatcher>().CaughtPassenger();
            }
        }
        
        if(other.gameObject.CompareTag("NearDropOff") && !caught)
        {
            StopAllCoroutines();
            StartCoroutine(InjuredAfterDelay(1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the passenger leaves the DropOff area, freeze them immediately
        if (other.gameObject.CompareTag("DropOff"))
        {
            FreezePassenger();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Passenger"))
            return;

        if (!collided && !caught)
        {
            collided = true;
            StartCoroutine(LostAfterDelay(1f));
        }
    }

    private IEnumerator InjuredAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!caught)
        {
            caught = true;
            FindAnyObjectByType<BusPassengers>().InjuredPassenger();
        }
    }

    private IEnumerator LostAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!caught)
        {
            caught = true;
            FindAnyObjectByType<BusPassengers>().LostPassenger();
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
        FreezePassenger();
    }

    private void FreezePassenger()
    {
        foreach (Rigidbody rb in passengerRigidbodies)
        {
            // Set velocity and angular velocity before making the Rigidbody kinematic
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // After setting the velocity and angular velocity, make the Rigidbody kinematic
                rb.isKinematic = true;
            }
        }
    }
}
