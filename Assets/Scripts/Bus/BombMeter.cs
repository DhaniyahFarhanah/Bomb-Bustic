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

    [Header("Bomb")]
    public TextMeshProUGUI bombTimerText;
    public ParticleSystem sparks;
    public Animator bombAnim;

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
            BombCountdown();
        }
        else
        {
            BombReset();
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

    private void BombCountdown()
    {
        if (countdownTimer <= 0f)
        {
            if(GetComponent<BusAudioHandler>().sfxIsLooping == true)
            {
                GetComponent<BusAudioHandler>().StopSFXLoop();
            }
            GetComponent<BusAudioHandler>().Play(GetComponent<BusAudioHandler>().BombExplosion);
            GetComponent<BusAudioHandler>().PlayOneShotSFX(GetComponent<BusAudioHandler>().BombDeadZone);

            countdownTextUI.text = "Countdown: BOOM!";
            bombTimerText.text = "BOOM!";
            countdownTimer = 0f;
        }
        else
        {
            countdownTimer -= Time.deltaTime;

            // If the countdown text is not active, make it active
            if (!countdownTextUI.gameObject.activeSelf)
            {
                countdownTextUI.gameObject.SetActive(true);
            }
            countdownTextUI.text = "Countdown: " + countdownTimer.ToString("F1") + "s\nToo Slow!";

            bombTimerText.text = countdownTimer.ToString("00.00");
            bombTimerText.color = Color.red;
            bombAnim.SetBool("Pulse", true);
            sparks.Play(true);

            // Pulse expand the BombImage 
            float pulseValue = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            BombImage.transform.localScale = Vector3.Lerp(bombOrginalScale, bombExpandScale, pulseValue);

            if(GetComponent<BusAudioHandler>().sfxIsLooping == false)
            {
                GetComponent<BusAudioHandler>().PlaySFXLoop(GetComponent<BusAudioHandler>().BombRedZone);
            }
        }
    }

    private void BombReset()
    {
        // Smoothly return the BombImage scale to the original scale when speed is above minSpeed
        countdownTimer = bombBuffer;
        if (!countdownTextUI.gameObject.activeSelf)
        {
            countdownTextUI.gameObject.SetActive(false);
        }

        bombTimerText.text = "SAFE";
        bombTimerText.color = Color.green;
        bombAnim.SetBool("Pulse", false);
        sparks.Stop(true);

        BombImage.color = Color.white;
        BombImage.transform.localScale = Vector3.Lerp(BombImage.transform.localScale, bombOrginalScale, Time.deltaTime * pulseSpeed);

        if(GetComponent<BusAudioHandler>().sfxIsLooping == true)
        {
            GetComponent<BusAudioHandler>().StopSFXLoop();
        }
    }

    public void BombPhysical()
    {

    }

}
