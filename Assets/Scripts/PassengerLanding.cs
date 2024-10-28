using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerLanding : MonoBehaviour
{
    public int PassengerID;
    private enum PassengerState
    {
        Healthy,
        Delivered,
        Injured,
        Lost,
    }
    private PassengerState passengerState = PassengerState.Healthy;
    private bool collided = false;
    private Rigidbody[] passengerRigidbodies;
    private bool done = false;
    private bool caught = false;

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
            if (other.gameObject.GetComponent<PassengerCatcher>().CheckCapacity())
            {
                // Mark as delivered and prevent further physics interactions
                passengerState = PassengerState.Delivered;
                StopAllCoroutines();
                StartCoroutine(SlowAndFreezePassenger());
                if (!done)
                {
                    done = true;
                    FindAnyObjectByType<BusPassengers>().DeliveredPassenger(PassengerID);
                    other.gameObject.GetComponent<PassengerCatcher>().CaughtPassenger();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the passenger leaves the DropOff area, freeze them immediately
        if (other.gameObject.CompareTag("DropOff") && passengerState == PassengerState.Delivered)
        {
            FreezePassenger();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!collided && passengerState != PassengerState.Delivered)
        {
            collided = true;
            StartCoroutine(CheckIfStopped());
        }
    }

    private IEnumerator CheckIfStopped()
    {
        bool isMoving = true;

        // Keep checking if the passenger has stopped moving
        while (isMoving)
        {
            isMoving = false; // Assume the passenger is stopped

            foreach (Rigidbody rb in passengerRigidbodies)
            {
                // Check if any rigidbody has a velocity or angular velocity above a threshold
                if (rb.velocity.magnitude > 1f || rb.angularVelocity.magnitude > 1f)
                {
                    isMoving = true; // The passenger is still moving
                    break;
                }
            }

            yield return null;
        }

        // Once the passenger has stopped moving, decide the final state
        if (passengerState == PassengerState.Injured)
        {
            if (!done)
            {
                done = true;
                FindAnyObjectByType<BusPassengers>().InjuredPassenger(PassengerID);
            }
        }
        else if (passengerState == PassengerState.Healthy) // Ensure it's not delivered
        {
            if (!done)
            {
                done = true;
                FindAnyObjectByType<BusPassengers>().LostPassenger(PassengerID);
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

    public bool GetCaught()
    {
        return caught;
    }

    public void SetCaught(bool _bool)
    {
        caught = _bool;
    }
}
