using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScaledBombSystem : MonoBehaviour
{
    [Header("Instantiates")]
    public TMP_Text bombTextOnBus;
    private Vehicle bus;
    private bool firstStart;    //for waiting for movement to start bomb tick
    private float busSpeed;
    private float secTimer;
    [SerializeField] Transform digitalDisplay;

    [Header("Bomb Values")]
    [SerializeField] int startBombTime;
    [SerializeField][Range(0,100)] int slowSpeedRange;
    [SerializeField][Range(0, 100)] int midSpeedRange;
    [SerializeField] int slowTickRate;
    [SerializeField] int midTickRate;
    [SerializeField] int fastTickRate;

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
}
