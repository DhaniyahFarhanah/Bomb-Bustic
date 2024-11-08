using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScaledBombSystem : MonoBehaviour
{
    [Header("Instantiates")]
    public TMP_Text bombTextOnBus;
    private Vehicle bus;
    private bool firstStart;    //for waiting for movement to start bomb tick
    private float busSpeed;
    private float secTimer;
    [SerializeField] Transform digitalDisplay;

    [Header("Speedometer UI Stuff")]
    [SerializeField] UIManager UIManger;
    [SerializeField] Image speedometerSlowImg;
    [SerializeField] Image speedometerFastImg;
    [SerializeField] Image speedometerSlowBackingImg;
    [SerializeField] Image speedometerFastBackingImg;
    [SerializeField] TMP_Text speedometerSlowRateText;
    [SerializeField] TMP_Text speedometerMidRateText;
    [SerializeField] TMP_Text speedometerFastRateText;
    [SerializeField] TMP_Text speedMaxText;
    [SerializeField] Image needle;
    private float maxZRotationNeedle;
    private float minZRotationNeedle;

    [Header("Bomb Values")]
    [SerializeField] int startBombTime;
    [SerializeField][Range(0,100)] int slowSpeedRange;
    [SerializeField][Range(0, 100)] int midSpeedRange;
    [SerializeField] int slowTickRate;
    [SerializeField] int midTickRate;
    [SerializeField] int fastTickRate;
    private float maxSpeed;

    [Header("Visual Aid")]
    private Vector3 orgPosOfDigitalDisplay;
    private bool shake = false;
    private float shakeDuration;
    [SerializeField] private float shakeSpeed;
    [Range(0.00f, 0.05f)][SerializeField] private float shakeIntensity;
    [Range(0.00f, 0.05f)][SerializeField] private float slowShakeIntensity;
    [Range(0.00f, 0.05f)][SerializeField] private float midShakeIntensity;
    [Range(0.00f, 0.05f)][SerializeField] private float fastShakeIntensity;

    [Header("BombStuff")]
    [SerializeField] int currentTimer;

    // Start is called before the first frame update
    void Start()
    {
        firstStart = true;
        shake = true;
        secTimer = 0;
        currentTimer = startBombTime;
        bus = gameObject.GetComponent<Vehicle>();
        orgPosOfDigitalDisplay = digitalDisplay.localPosition;
        maxSpeed = bus.Settings.MaxSpeed;

        speedometerSlowImg.fillAmount = slowSpeedRange / bus.Settings.MaxSpeed;
        speedometerSlowBackingImg.fillAmount = speedometerSlowImg.fillAmount + 0.02f;
        speedometerFastImg.fillAmount = (bus.Settings.MaxSpeed - midSpeedRange) / bus.Settings.MaxSpeed;
        speedometerFastBackingImg.fillAmount = speedometerFastImg.fillAmount + 0.02f;

        //tick rate
        speedometerSlowRateText.text = "-" + (slowTickRate).ToString();
        speedometerMidRateText.text = "-" + (midTickRate).ToString();
        speedometerFastRateText.text = "-" + (fastTickRate).ToString();

        //speed limits
        speedMaxText.text = ((int)maxSpeed).ToString();

        maxZRotationNeedle = needle.transform.localEulerAngles.z;
        minZRotationNeedle = maxZRotationNeedle - 180f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstStart)
        {
            if(currentTimer <= 0)
            {
                //gameover
                bombTextOnBus.text = "BOOM!";
            }
            else
            {
                CalculateBombReduction();
            }
        }
        else
        {
            digitalDisplay.localPosition = orgPosOfDigitalDisplay + Random.insideUnitSphere * shakeIntensity;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            firstStart = false;
        }

        if (shake)
        {
            if (shakeDuration > 0)
            {
                digitalDisplay.localPosition = orgPosOfDigitalDisplay + Random.insideUnitSphere * shakeIntensity;

                shakeDuration -= Time.deltaTime;
            }
            else if(shakeDuration <= 0)
            {
                digitalDisplay.localPosition = orgPosOfDigitalDisplay;
                shake = false;
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateNeedleRotation();
    }

    void CalculateBombReduction()
    {
        busSpeed = bus.Velocity.magnitude;


        if (secTimer < 1)
        {
            secTimer += Time.deltaTime;
        }

        else if (secTimer >= 1) //per 1 sec
        {
            float intensity = 0f;

            //when low speed
            if (busSpeed < slowSpeedRange)
            {
                currentTimer -= slowTickRate;
                intensity = slowShakeIntensity;
                bombTextOnBus.color = Color.red;
            }

            //when middle speed 
            else if (busSpeed > slowSpeedRange && busSpeed < midSpeedRange)
            {
                currentTimer -= midTickRate;
                intensity = midShakeIntensity;
                bombTextOnBus.color = Color.yellow;
            }

            //when fast
            else if (busSpeed > midSpeedRange)
            {
                currentTimer -= fastTickRate;
                intensity = fastShakeIntensity;
                bombTextOnBus.color = Color.green;
            }

            shake = true;
            shakeIntensity = intensity;
            shakeDuration = shakeSpeed;

            secTimer = 0;
        }

        bombTextOnBus.text = currentTimer.ToString();
    }

    void UpdateNeedleRotation()
    {
        float currentZAngle = needle.transform.localEulerAngles.z;
        float currentSpeedZ = (busSpeed / maxSpeed) * (minZRotationNeedle - maxZRotationNeedle);

        Debug.Log(currentSpeedZ);

        if(busSpeed > maxSpeed)
        {
            //keep needle pointing at max
            needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, minZRotationNeedle);
        }
        else if(busSpeed <= maxSpeed)
        {
            needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, currentSpeedZ);
        }
       
    }
}
