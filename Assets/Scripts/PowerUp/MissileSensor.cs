using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSensor : MonoBehaviour
{
    [SerializeField] Missile mis;

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (hitObject.GetComponent<ObstacleType>() != null && !hitObject.CompareTag("Player") && !hitObject.CompareTag("Indestructable") && !hitObject.CompareTag("DropOff"))
        {
            //home in babyyy
            mis.target = hitObject;
            mis.homeIn = true;
        }
    }
}
