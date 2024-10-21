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
    public TextMeshProUGUI speedTextUI;
    public TextMeshProUGUI countdownTextUI;

    private Vehicle bus;
    private Rigidbody rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bus = GetComponent<Vehicle>();

        countdownTimer = bombBuffer;
        bombMeterSlider.maxValue = bus.Settings.MaxSpeed;
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
        speedTextUI.text = "Speed: " + Mathf.FloorToInt(currentSpeed).ToString();
    }

    private void BombLogic()
    {
        if (currentSpeed <= minSpeed)
        {
            countdownTimer -= Time.deltaTime; 
            countdownTextUI.text = "Countdown: " + Mathf.FloorToInt(countdownTimer).ToString() + "s";
            if (!countdownTextUI.gameObject.activeSelf)
            {
                countdownTextUI.gameObject.SetActive(true);
            }
        }
        else if (countdownTimer != bombBuffer)
        {
            countdownTimer = bombBuffer;
            countdownTextUI.gameObject.SetActive(false);
        }

        if (countdownTimer <= 0f)
        {
            countdownTextUI.text = "Countdown: BOOM!";
        }
    }
}
