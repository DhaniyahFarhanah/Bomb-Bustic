using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType
{
    Empty,
    Turret,
    Hack,
    Nitro,
    EnergyPulse
}

public class PickUpHandler : MonoBehaviour
{
    private PickUpType currentPickUp;
    

    // Start is called before the first frame update
    void Start()
    {
        //Default empty on start up
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
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                //activate pickup
                ActivatePickup(currentPickUp);

            }
        }
    }

    void ActivatePickup(PickUpType type)
    {
        switch (type)
        {

        }
        //navigate to correct mechanic
        currentPickUp = PickUpType.Empty;
    }

    void ActivateTurret()
    {

    }

    void ActivateHack()
    {

    }

    void ActivateNitro()
    {

    }

    void ActivateEnergyPulse()
    {
        //For this powerup, it sends out a "pulse" through particle that stops the speed of all cars within a range for a certain amount of time
    }
}
