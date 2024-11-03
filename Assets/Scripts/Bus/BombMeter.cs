using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArcadeVehicleController;

public class BombMeter : MonoBehaviour
{
    [Header("BombSettings")]
    public float minSpeed = 5f;
    public float bombBuffer = 5f;
    private float countdownTimer;

    [Header("UI")]
    public Slider bombMeterSlider;
    public TextMeshProUGUI countdownTextUI;
    public Image BombImage;
    public Image BombPointer;
    public RectTransform crashFill;
    public float pulseSpeed = 2f;
    public float bombExpand;
    private Vector3 bombExpandScale;
    private Vector3 bombOrginalScale;
    public float BombPointerMinAngle = 20f;
    public float BombPointerMaxAngle = -80f;

    private Vehicle bus;
    private Rigidbody rb;
    private float currentSpeed;
    private float maxSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bus = GetComponent<Vehicle>();

        bombOrginalScale = transform.localScale;
        bombExpandScale = bombOrginalScale * bombExpand;

        countdownTimer = bombBuffer;
        maxSpeed = bus.Settings.MaxSpeed;
        bombMeterSlider.maxValue = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = rb.velocity.magnitude;

        UIUpdate();
        BombLogic();
        BombPointerAngle();
    }

    private void UIUpdate()
    {
        bombMeterSlider.value = currentSpeed;
        bombMeterSlider.fillRect.anchorMin = new Vector2(0f, 1 - (minSpeed / maxSpeed));
    }

    private void BombLogic()
    {
        if (currentSpeed <= minSpeed)
        {
            countdownTimer -= Time.deltaTime;
            countdownTextUI.text = "Countdown: " + Mathf.FloorToInt(countdownTimer).ToString() + "s\nToo Slow!";

            // If the countdown text is not active, make it active
            if (!countdownTextUI.gameObject.activeSelf)
            {
                countdownTextUI.gameObject.SetActive(true);
            }

            // Pulse the BombImage red and expand
            float pulseValue = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            BombImage.transform.localScale = Vector3.Lerp(bombOrginalScale, bombExpandScale, pulseValue);
        }
        else
        {
            // Smoothly return the BombImage scale to the original scale when speed is above minSpeed
            countdownTimer = bombBuffer;
            countdownTextUI.gameObject.SetActive(false);

            BombImage.transform.localScale = Vector3.Lerp(BombImage.transform.localScale, bombOrginalScale, Time.deltaTime * pulseSpeed);
        }

        // Handle bomb explosion
        if (countdownTimer <= 0f)
        {
            countdownTextUI.text = "Countdown: BOOM!";
            countdownTimer = 0f;
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void SetCrashSpeedUI(float minCrashSpeed)
    {
        // Calculate the height as a proportion of the slider's full height
        float proportionalHeight = bombMeterSlider.GetComponent<RectTransform>().rect.height * (1 - (minCrashSpeed / maxSpeed));

        // Set the new height to crashFill
        crashFill.sizeDelta = new Vector2(crashFill.sizeDelta.x, proportionalHeight);
    }

    private void BombPointerAngle()
    {
        float maxDiff = BombPointerMaxAngle - BombPointerMinAngle;

        if (currentSpeed > minSpeed)
        {
            // Smoothly transition the bomb pointer back to the maximum angle when speed is above minSpeed
            float currentZAngle = BombPointer.transform.localEulerAngles.z;

            // Ensure the angle wraps correctly by adjusting values within 0-360 range
            if (currentZAngle > 180) currentZAngle -= 360;

            float bombPointerAngle = Mathf.Lerp(currentZAngle, BombPointerMaxAngle, Time.deltaTime * pulseSpeed);

            BombPointer.transform.localEulerAngles = new Vector3(
                BombPointer.transform.localEulerAngles.x,
                BombPointer.transform.localEulerAngles.y,
                bombPointerAngle
            );
        }
        else
        {
            // Interpolate angle based on the countdown timer for the "too slow" warning
            float bombPointerAngle = Mathf.Lerp(BombPointerMaxAngle, BombPointerMinAngle, 1 - countdownTimer / bombBuffer);

            BombPointer.transform.localEulerAngles = new Vector3(
                BombPointer.transform.localEulerAngles.x,
                BombPointer.transform.localEulerAngles.y,
                bombPointerAngle
            );
        }
    }
}
