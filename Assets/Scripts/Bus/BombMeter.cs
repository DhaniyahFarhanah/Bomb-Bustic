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
    public RectTransform crashFill;
    public float pulseSpeed = 2f;
    public float bombExpand;
    private Vector3 bombExpandScale;
    private Vector3 bombOrginalScale;

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

            // Pulse the BombImage red
            float pulseValue = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            //BombImage.color = Color.Lerp(Color.white, Color.red, pulseValue); // Transition from white to red
            BombImage.transform.localScale = Vector3.Lerp(bombOrginalScale, bombExpandScale, pulseValue); // Transition from white to red
        }
        else
        {
            // Reset the timer when speed is above the minimum
            if (countdownTimer != bombBuffer)
            {
                countdownTimer = bombBuffer;
                countdownTextUI.gameObject.SetActive(false);
            }

            // Reset the BombImage color to its original color (white)
            //BombImage.color = Color.white;
            BombImage.transform.localScale = bombOrginalScale;
        }

        // Handle bomb explosion
        if (countdownTimer <= 0f)
        {
            countdownTextUI.text = "Countdown: BOOM!";
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

}
