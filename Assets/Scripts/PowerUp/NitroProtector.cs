using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroProtector : MonoBehaviour
{
    [SerializeField] GameObject SmokeCloud;

    private void OnTriggerStay(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (hitObject.GetComponentInParent<RougeAI>())
        {
            Instantiate(SmokeCloud, hitObject.transform.position, Quaternion.identity);
            hitObject.GetComponentInParent<RougeAI>().SelfDestruct();
            return;
        }

        if (hitObject.GetComponentInParent<BasicAI>())
        {
            Instantiate(SmokeCloud, hitObject.transform.position, Quaternion.identity);
            Destroy(hitObject.GetComponentInParent<BasicAI>().gameObject);
            return;
        }

        if (hitObject.GetComponent<ObstacleType>() != null && !hitObject.CompareTag("Player") && !hitObject.CompareTag("Indestructable") && !hitObject.CompareTag("DropOff"))
        {
            Instantiate(SmokeCloud, hitObject.transform.position, Quaternion.identity);
            Destroy(hitObject);
        }
    }
}
