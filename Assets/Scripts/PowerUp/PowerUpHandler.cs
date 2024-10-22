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
    [SerializeField] GameObject Bus;
    [SerializeField] float currentTimer;
    [SerializeField] bool activated;
    [SerializeField] Image powerUpImage;

    //Turret activates Turret powerup
    [Header("Turret PowerUp")]
    [SerializeField] GameObject Turret;
    [SerializeField] DrivingCameraController cam;
    [SerializeField] float Cooldown;

    //Hack makes the bomb limit to 0 for a while
    [Header("Hack PowerUp")]
    Vehicle busValues;

    //speeds up bus (maybe make acceleration 100 or smth)
    [Header("Nitro PowerUp")]
    [SerializeField] float speedUpMulti;
    [SerializeField] float addSpeedCap;

    //Stops the speed of every movable object in a large range. Idk yet
    [Header("Energy Pulse")]
    [SerializeField] float range;
    

    // Start is called before the first frame update
    void Start()
    {
        //Default empty on start up
        busValues = Bus.GetComponent<Vehicle>();

        activated = false;
        currentPickUp = PickUpType.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        PickUpInput();
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
                powerUpImage.sprite = null;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                //activate pickup
                activated = true;
                ActivatePickup(currentPickUp);

            }
        }
    }

    void ActivatePickup(PickUpType type)
    {
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
                break;
            case PickUpType.Nitro:
                ActivateNitro();
                break;
            case PickUpType.EnergyPulse:
                ActivateEnergyPulse();
                break;
        }
        //navigate to correct mechanic
        currentPickUp = PickUpType.Empty;
    }

    public void ReceivePickup(Sprite img, PickUpType type)
    {
        currentPickUp = type;
        powerUpImage.gameObject.SetActive(true);
        powerUpImage.sprite = img;
    }

    void Deactivate()
    {

    }

    void ActivateTurret()
    {
        Debug.Log("Turret");
    }

    void ActivateHack()
    {
        Debug.Log("Hack");
    }

    void ActivateNitro()
    {
        Debug.Log("Nitro");
    }

    void ActivateEnergyPulse()
    {
        Debug.Log("EnergyPulse");
        //For this powerup, it sends out a "pulse" through particle that stops the speed of all cars within a range for a certain amount of time
    }
}
