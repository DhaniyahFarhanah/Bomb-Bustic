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
    [HideInInspector] public bool objectiveFinished;

    [Header("Speedometer UI Stuff")]
    public bool freeze;
    [SerializeField] GameObject freezeNeedle;
    [SerializeField] GameObject freezeSpeedometer;
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
    [SerializeField][Range(0, 100)] int slowSpeedRange;
    [SerializeField][Range(0, 100)] int midSpeedRange;
    [SerializeField] int slowTickRate;
    [SerializeField] int midTickRate;
    [SerializeField] int fastTickRate;
    private float maxSpeed;
    private float newTime;

    [Header("Visual Aid")]
    private Vector3 orgPosOfDigitalDisplay;
    private bool shake = false;
    private float shakeDuration;
    [SerializeField] private float shakeSpeed;
    [Range(0.00f, 0.05f)][SerializeField] private float shakeIntensity;
    [Range(0.00f, 0.05f)][SerializeField] private float slowShakeIntensity;
    [Range(0.00f, 0.05f)][SerializeField] private float midShakeIntensity;
    [Range(0.00f, 0.05f)][SerializeField] private float fastShakeIntensity;
    [SerializeField] private Color addingColor;
    [SerializeField] private Color subtractColor;

    [Header("BombStuff")]
    [SerializeField] int currentTimer;

    BusAudioHandler audio;

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
        audio = gameObject.GetComponent<BusAudioHandler>();

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
            if (currentTimer <= 0)
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

        //any input to move bus
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space))
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

        if (secTimer < 1 && !freeze && !objectiveFinished)
        {
            secTimer += Time.deltaTime;
        }

        else if (secTimer >= 1 && !freeze && !objectiveFinished) //per 1 sec
        {
            float intensity = 0f;

            //when low speed
            if (busSpeed < slowSpeedRange)
            {
                currentTimer -= slowTickRate;
                intensity = slowShakeIntensity;
                bombTextOnBus.color = Color.red;

                audio.PlayOneShotSFX(audio.fastTick);
            }

            //when middle speed 
            else if (busSpeed > slowSpeedRange && busSpeed < midSpeedRange)
            {
                currentTimer -= midTickRate;
                intensity = midShakeIntensity;
                bombTextOnBus.color = Color.yellow;
                audio.PlayOneShotSFX(audio.midTick);
            }

            //when fast
            else if (busSpeed > midSpeedRange)
            {
                currentTimer -= fastTickRate;
                intensity = fastShakeIntensity;
                bombTextOnBus.color = Color.green;
                audio.PlayOneShotSFX(audio.slowTick);
            }

            shake = true;
            shakeIntensity = intensity;
            shakeDuration = shakeSpeed;

            secTimer = 0;
        }

        if (freeze)
        {
            bombTextOnBus.text = ">:C";
            bombTextOnBus.color = Color.cyan;

            shake = true;
            shakeIntensity = midShakeIntensity;
        }

        else
        {
            bombTextOnBus.text = currentTimer.ToString();
        }

        if (objectiveFinished)
        {

            if(currentTimer < newTime)
            {
                currentTimer += 2;
                audio.PlayOneShotSFX(audio.fastTick);
                bombTextOnBus.color = addingColor;
            }
            else if(currentTimer > newTime)
            {
                currentTimer--;
                audio.PlayOneShotSFX(audio.slowTick);
                bombTextOnBus.color = subtractColor;
            }

            else if (currentTimer == newTime)
            {
                objectiveFinished = false;
            }
        }
    }

    void UpdateNeedleRotation()
    {
        float currentZAngle = needle.transform.localEulerAngles.z;
        float currentSpeedZ = 0f;

        if(busSpeed > maxSpeed && !freeze)
        {
            //keep needle pointing at max
            needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, minZRotationNeedle);
        }

        if(busSpeed < 0f && !freeze)
        {
            needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, maxZRotationNeedle);
        }

        else if(busSpeed <= maxSpeed)
        {
            if (freeze)
            {
                freezeNeedle.SetActive(true);
                freezeSpeedometer.SetActive(true);
                currentSpeedZ = needle.transform.localEulerAngles.z;
            }
            else
            {
                freezeNeedle.SetActive(false);
                freezeSpeedometer.SetActive(false);
                currentSpeedZ = (busSpeed / maxSpeed) * (minZRotationNeedle - maxZRotationNeedle);
            }

            needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, currentSpeedZ);
        }
    }

    public void AddTime(int time)
    {
        objectiveFinished = true;
        newTime = currentTimer + time;
    }
}
