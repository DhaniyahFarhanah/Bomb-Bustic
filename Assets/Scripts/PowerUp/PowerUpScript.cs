using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PowerUpScript : MonoBehaviour
{
    [SerializeField] bool setType;
    [SerializeField] PickUpType type;

    [SerializeField] Sprite TurretImage;
    [SerializeField] Sprite HackImage;
    [SerializeField] Sprite NitroImage;
    [SerializeField] Sprite EnergyPulseImage;
    Sprite givenImage;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.gameObject;

        if(hit.GetComponent<PowerUpHandler>() != null)
        {
            if (!hit.GetComponent<PowerUpHandler>().activated)
            {
                if (!setType)
                {
                    if (hit.CompareTag("Player"))
                    {
                        PickUpType powerUp = (PickUpType)Random.Range(1, 5);

                        switch (powerUp)
                        {
                            case PickUpType.Turret:
                                givenImage = TurretImage;
                                break;
                            case PickUpType.Hack:
                                givenImage = HackImage;
                                break;
                            case PickUpType.Nitro:
                                givenImage = NitroImage;
                                break;
                            case PickUpType.EnergyPulse:
                                givenImage = EnergyPulseImage;
                                break;
                            default:
                                Debug.Log("error!");
                                break;
                        }

                        hit.GetComponent<PowerUpHandler>().ReceivePickup(givenImage, powerUp);
                        Destroy(this.gameObject);
                    }
                }

                else if (setType)
                {
                    if (hit.CompareTag("Player"))
                    {
                        switch (type)
                        {
                            case PickUpType.Turret:
                                givenImage = TurretImage;
                                break;
                            case PickUpType.Hack:
                                givenImage = HackImage;
                                break;
                            case PickUpType.Nitro:
                                givenImage = NitroImage;
                                break;
                            case PickUpType.EnergyPulse:
                                givenImage = EnergyPulseImage;
                                break;
                            default:
                                Debug.Log("error!");
                                break;
                        }

                        hit.GetComponent<PowerUpHandler>().ReceivePickup(givenImage, type);
                        Destroy(this.gameObject);
                    }

                }
            }
        }
        
        
        
    }
}
