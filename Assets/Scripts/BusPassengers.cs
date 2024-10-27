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

    [Header("Turret Settings")]
    [SerializeField] private Vector3 exitOffset;
    [SerializeField] private float passengerExitForce = 500f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float slowMotionTime = 10f;
    [SerializeField] private float slowMotionScale = 0.2f;
    [SerializeField] private float slowMotionTransitionSpeed = 3f;
    private bool passengerEjectionActive = false;
    private bool slowMoActive = false;
    private float slowMotionElapsedTime;
    private float elapsedTime;

    [Header("UI")]
    [SerializeField] private PassengerInfoUI passengerIcons;
    [SerializeField] private GameObject shootingObject;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject ShootingInfo;
    [SerializeField] private Slider shootingSlidingTimerUI;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private float crosshairScaleDuration = 0.1f;
    [SerializeField] private float crosshairMaxScale = 1.5f;
    [SerializeField] private float crosshairSpinDuration = 0.5f;
    [SerializeField] private Color slowMotionColor = Color.blue;
    [SerializeField] private Color normalColor = Color.green;
    private Vector3 originalCrosshairScale;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSettings();
    }

    void Update()
    {
        HandleInput();
        UpdateCrosshairColor();
        HandleSlowMotion();
        UpdateSlowMotionTimer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ShootZone")
        {
            EnablePassengerEjection(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ShootZone")
        {
            EnablePassengerEjection(false);
        }
    }

    private void InitializeSettings()
    {
        passengerCurrent = passengerTotal;
        passengerIcons.InitPassengers(passengerTotal);
        UpdatePassengerText();

        EnablePassengerEjection(false);
        originalCrosshairScale = crosshairUI.transform.localScale;
        shootingSlidingTimerUI.maxValue = slowMotionTime;
        sliderFillImage.color = normalColor;
    }

    private void HandleInput()
    {
        if (passengerCurrent > 0 && elapsedTime <= 0f && passengerEjectionActive)
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse button
            {
                elapsedTime = fireRate;
                ShootPassenger();
                StartCoroutine(AnimateCrosshairScale());
            }
        }
    }

    private void ShootPassenger()
    {
        // Create a ray from the center of the camera's viewport
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen (viewport coordinates)

        // Calculate the position based on the exit offset, using the bus's local orientation
        Vector3 exitPosition = transform.TransformPoint(exitOffset);

        // Instantiate the passenger at the calculated exit position
        GameObject passenger = Instantiate(passengerPrefab, exitPosition, Quaternion.identity);

        // Apply horizontal shooting direction force and add vertical force
        Vector3 shootingDirection = ray.direction * passengerExitForce;
        Vector3 verticalForce = Vector3.up * passengerExitForce * 0.5f; // Add vertical force (0.5 multiplier for balance)

        // Combine both forces (forward and upward)
        Vector3 totalForce = shootingDirection + verticalForce;

        ApplyPassengerPhysics(passenger, totalForce);

        // Decrease the number of current passengers
        --passengerCurrent;
        UpdatePassengerText();
    }

    private void ApplyPassengerPhysics(GameObject passenger, Vector3 totalForce)
    {
        // Get all the Rigidbody components of the ragdoll (the passenger object and its children)
        Rigidbody[] passengerRigidbodies = passenger.GetComponentsInChildren<Rigidbody>();

        // Get the Rigidbody of the bus (the object this script is attached to)
        Rigidbody busRb = GetComponent<Rigidbody>();

        if (passengerRigidbodies.Length > 0 && busRb != null)
        {
            // Set the velocity of the ragdoll's Rigidbody to match the bus's current velocity
            foreach (Rigidbody rb in passengerRigidbodies)
            {
                rb.velocity = busRb.velocity;
            }

            // Apply the total force (forward + vertical) to the root Rigidbody of the ragdoll
            StartCoroutine(ApplyForceGradually(passengerRigidbodies[0], totalForce));
        }
    }

    private IEnumerator ApplyForceGradually(Rigidbody passengerRb, Vector3 force)
    {
        int steps = (int)(passengerExitForce / 100f); // Number of frames to apply force over
        Vector3 incrementalForce = force / steps;

        for (int i = 0; i < steps; i++)
        {
            passengerRb.AddForce(incrementalForce, ForceMode.Impulse);
            yield return new WaitForFixedUpdate();
        }
    }

    private void HandleSlowMotion()
    {
        if (Input.GetMouseButton(1) && passengerEjectionActive) // Right mouse button held
        {
            ApplySlowMotion();
            shootingSlidingTimerUI.gameObject.SetActive(true);
        }
        else
        {
            ResetTimeScale();
            shootingSlidingTimerUI.gameObject.SetActive(false);
        }
    }

    private void ApplySlowMotion()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionScale, Time.deltaTime * slowMotionTransitionSpeed);
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Maintain consistent fixed time step during slow motion
        sliderFillImage.color = slowMotionColor;
        slowMoActive = true;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, Time.deltaTime * slowMotionTransitionSpeed);
        Time.fixedDeltaTime = 0.02f; // Reset to normal fixed time step
        sliderFillImage.color = normalColor;
        slowMoActive = false;
    }

    private void UpdateSlowMotionTimer()
    {
        if (slowMotionElapsedTime > 0 && slowMoActive)
        {
            slowMotionElapsedTime -= Time.unscaledDeltaTime;
            shootingSlidingTimerUI.value = slowMotionElapsedTime;
        }
    }

    private void UpdateCrosshairColor()
    {
        if (elapsedTime > 0f)
        {
            elapsedTime -= Time.deltaTime;
            crosshairUI.GetComponent<Image>().color = Color.black;
        }
        else
        {
            crosshairUI.GetComponent<Image>().color = Color.white;
        }
    }

    private IEnumerator AnimateCrosshairScale()
    {
        yield return ScaleCrosshair(1f, crosshairMaxScale, crosshairScaleDuration);
        yield return ScaleCrosshair(crosshairMaxScale, 1f, crosshairScaleDuration);
        crosshairUI.transform.localScale = originalCrosshairScale;
    }

    private IEnumerator ScaleCrosshair(float startScale, float endScale, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, time / duration);
            crosshairUI.transform.localScale = originalCrosshairScale * scale;
            yield return null;
        }
    }

    private IEnumerator SpinCrosshair()
    {
        float elapsed = 0f;
        while (elapsed < crosshairSpinDuration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 360f, elapsed / crosshairSpinDuration);
            crosshairUI.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
        crosshairUI.transform.rotation = Quaternion.identity;
    }

    public void EnablePassengerEjection(bool enabled)
    {
        passengerEjectionActive = enabled;
        shootingObject.SetActive(passengerEjectionActive);
        if (passengerEjectionActive)
        {
            slowMotionElapsedTime = slowMotionTime;
            StartCoroutine(AnimateCrosshairScale());
            StartCoroutine(SpinCrosshair());
            StartCoroutine(RemoveShootingInfoAfterDelay());
        }
    }

    public void DeliveredPassenger()
    {
        ++passengerDelivered;
        passengerIcons.DeliveredPassenger();
        UpdatePassengerText();
    }

    public void InjuredPassenger()
    {
        ++passengerInjured;
        passengerIcons.InjuredPassenger();
        UpdatePassengerText();
    }

    public void LostPassenger()
    {
        ++passengerLost;
        passengerIcons.LostPassenger();
        UpdatePassengerText();
    }

    private void UpdatePassengerText()
    {
        passengerIcons.UpdatePassengerInfoText(passengerCurrent, passengerDelivered, passengerInjured, passengerLost);
    }

    private IEnumerator RemoveShootingInfoAfterDelay()
    {
        ShootingInfo.SetActive(true);
        yield return new WaitForSeconds(5f);
        ShootingInfo.SetActive(false);
    }
}
