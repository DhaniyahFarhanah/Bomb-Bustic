using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PickUpType
{
    Empty,
    Turret,
    Hack,
    Nitro,
    EnergyPulse
}

public class PowerUpHandler : MonoBehaviour
{
    public PickUpType currentPickUp;
    [SerializeField] Animator PickUpAnimator;
    [SerializeField] GameObject Bus;
    [SerializeField] float currentTimer;
    [SerializeField] float imageTimer;
    public bool activated;
    [SerializeField] Image powerUpImage;
    public Image backingImage;
    [SerializeField] Sprite emptyImage;
    private BusAudioHandler busAudioHandler;

    //Turret activates Turret powerup
    [Header("Turret PowerUp")]
    [SerializeField] GameObject turret;
    [SerializeField] DrivingCameraController cam;
    [SerializeField] float turretCooldown;

    //Hack makes the bomb limit to 0 for a while
    [Header("Hack PowerUp")]
    [SerializeField] float hackCooldown;
    BombMeter bombMeter;
    Vehicle busValues;
    float bombMeterNorm;

    //speeds up bus (maybe make acceleration 100 or smth)
    [Header("Nitro PowerUp")]
    [SerializeField] float nitroCooldown;
    [SerializeField] float addFov;
    [SerializeField] GameObject NitroProtector;

    //Stops the speed of every movable object in a large range. Idk yet
    [Header("Energy Pulse")]
    [SerializeField] GameObject empPulse;
    [SerializeField] float energyPulseCooldown;
    

    // Start is called before the first frame update
    void Start()
    {
        backingImage.color = Color.grey;

        //Default empty on start up
        busValues = Bus.GetComponent<Vehicle>();
        bombMeter = Bus.GetComponent<BombMeter>();

        bombMeterNorm = bombMeter.minSpeed;
        activated = false;
        currentPickUp = PickUpType.Empty;

        busAudioHandler = GetComponent<BusAudioHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        PickUpInput();

        if(currentTimer >= 0 && activated)
        {
            currentTimer -= Time.deltaTime;

            backingImage.fillAmount = backingImage.fillAmount = (currentTimer / imageTimer) * 1f;

            if(backingImage.fillAmount < 0.6 && backingImage.fillAmount > 0.2)
            {
                backingImage.color = Color.yellow;
            }
            else if(backingImage.fillAmount < 0.2)
            {
                backingImage.color = Color.red;
            }
            else
            {
                backingImage.color= Color.green;
            }
        }

        else if(currentTimer <= 0 && activated)
        {
            Deactivate(currentPickUp);
            backingImage.color = Color.white;
            backingImage.fillAmount = 1f;
        }

        if(currentPickUp == PickUpType.Empty)
        {
            backingImage.color = Color.grey;
            PickUpAnimator.SetBool("Empty", true);
        }
        else
        {
            PickUpAnimator.SetBool("Empty", false);
        }
    }

    private void FixedUpdate()
    {
        
    }

    void PickUpInput()
    {
        if(currentPickUp != PickUpType.Empty)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //drop pickup
                currentPickUp = PickUpType.Empty;
                powerUpImage.sprite = emptyImage;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                GetComponent<BusPassengers>().ActivatePassengerEjectionMode(false);
                //activate pickup
                activated = true;
                ActivatePickup(currentPickUp);
            }
        }
    }

    void ActivatePickup(PickUpType type)
    {
        activated = true;
        PickUpAnimator.SetBool("Activate", true);

        switch (type)
        {
            case PickUpType.Empty: 
                Debug.Log("Empty");
                break;
            case PickUpType.Turret:
                ActivateTurret();
                break;
            case PickUpType.Hack:
                ActivateHack();
                busAudioHandler.PlayPriority(GetComponent<BusAudioHandler>().Emp);
                break;
            case PickUpType.Nitro:
                ActivateNitro();
                busAudioHandler.PlayPriority(GetComponent<BusAudioHandler>().Boost);
                break;
            case PickUpType.EnergyPulse:
                ActivateEnergyPulse();
                busAudioHandler.PlayPriority(GetComponent<BusAudioHandler>().Shockwave);
                break;
        }
        //navigate to correct mechanic
    }

    public void ReceivePickup(Sprite img, PickUpType type)
    {
        currentPickUp = type;
        powerUpImage.gameObject.SetActive(true);
        powerUpImage.sprite = img;
        backingImage.fillAmount = 1f;
    }

    void Deactivate(PickUpType type)
    {
        PickUpAnimator.SetBool("Activate", false);

        switch (type)
        {
            case PickUpType.Turret:
                turret.SetActive(false);
                cam.SetCameraMode(CameraModes.Normal);
                Cursor.visible = false;
                break;
            case PickUpType.Hack:
                bombMeter.minSpeed = bombMeterNorm;
                break;
            case PickUpType.Nitro:
                busValues.Nitro = false;
                cam.nitroFOVIncrease = 0f;
                NitroProtector.SetActive(false);
                gameObject.GetComponent<CollisionHandler>().enabled = true;
                break;
            case PickUpType.EnergyPulse:
                empPulse.SetActive(false);
                break;
        }

        activated = false;
        currentPickUp = PickUpType.Empty;
        powerUpImage.sprite = emptyImage;
        backingImage.fillAmount = 1f;
    }

    void ActivateTurret()
    {
        turret.SetActive(true);
        cam.SetCameraMode(CameraModes.Turret);
        currentTimer = turretCooldown;
        imageTimer = turretCooldown;
    }

    void ActivateHack()
    {
        //this powerup, you have a bar protection and the bomb wont have a limit anymore. Last for a certain amount of minutes
        Debug.Log("Hack");
        currentTimer = hackCooldown;
        imageTimer = hackCooldown;
        bombMeter.minSpeed = 0f;

    }

    void ActivateNitro()
    {
        Debug.Log("Nitro");
        gameObject.GetComponent<CollisionHandler>().enabled = false;
        busValues.Nitro = true;
        cam.nitroFOVIncrease = addFov;
        NitroProtector.SetActive(true);
        currentTimer = nitroCooldown;
        imageTimer = nitroCooldown;
    }

    void ActivateEnergyPulse()
    {
        Debug.Log("EnergyPulse");
        empPulse.SetActive(true);
        currentTimer = energyPulseCooldown;
        imageTimer = energyPulseCooldown;
        //For this powerup, it sends out a "pulse" through particle that stops the speed of all cars within a range for a certain amount of time
    }

    public void DeactivateCurrent()
    {
        if (activated)
            Deactivate(currentPickUp);
    }

}
