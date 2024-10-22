using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PowerUpScript : MonoBehaviour
{
    [SerializeField] Sprite TurretImage;
    [SerializeField] Sprite HackImage;
    [SerializeField] Sprite NitroImage;
    [SerializeField] Sprite EnergyPulseImage;
    Sprite givenImage;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        GameObject hit = other.gameObject;

        if (hit.gameObject.CompareTag("Player"))
        {
            PickUpType powerUp = (PickUpType)Random.Range(1, 4);

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

            Debug.Log("pick up power up");
            Destroy(this.gameObject);
        }
    }
}
