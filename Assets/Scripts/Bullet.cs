using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        GameObject hitObject = collision.gameObject;
        if (hitObject.GetComponentInParent<RougeAI>())
        {
            hitObject.GetComponentInParent<RougeAI>().SelfDestruct();
            return;
        }
    }
}
