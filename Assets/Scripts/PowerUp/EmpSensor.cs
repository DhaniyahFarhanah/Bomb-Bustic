using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        if(obj.GetComponentInParent<RougeAI>() != null)
        {
            obj.GetComponentInParent<RougeAI>().stop = true;
        }

        else if(obj.GetComponentInParent<AICarEngine>() != null)
        {
            obj.GetComponentInParent<AICarEngine>().stop = true;
        }

        else
        {
            return;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;

        if (obj.GetComponentInParent<RougeAI>() != null)
        {
            obj.GetComponentInParent<RougeAI>().stop = false;
        }

        else if (obj.GetComponentInParent<AICarEngine>() != null)
        {
            obj.GetComponentInParent<AICarEngine>().stop = false;
        }
        else
        {
            return;
        }
    }
}
