using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroProtector : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (hitObject.GetComponentInParent<RougeAI>())
        {
            hitObject.GetComponentInParent<RougeAI>().SelfDestruct();
            return;
        }

        if (hitObject.GetComponent<ObstacleType>() != null && !hitObject.CompareTag("Player") && !hitObject.CompareTag("Indestructable") && !hitObject.CompareTag("DropOff"))
        {
            Destroy(hitObject.transform.parent.gameObject);
        }
    }
}
