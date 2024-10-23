using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public LayerMask miniMask;
    private GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject g in allGameObjects) {
            if(g.layer ==  miniMask) {
                icon = g;
            }
        }
        
    }

    void Update() {
         GameObject target = GameObject.FindGameObjectWithTag("Player");
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Quaternion targetRot = target.transform.rotation;
        transform.rotation = Quaternion.Euler(90f, targetRot.eulerAngles.y, 0f);
        //icon.transform.rotation = Quaternion.Euler(Icon.eulerAngles.x , transform.eulerAngles.y, targetRot.eulerAngles.y);
    }
    
    /* void LateUpdate() {
        if(transform.parent != null) {
            float y = transform.parent.eulerAngles.y;
            Quaternion target = Quaternion.Euler(90f, y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
        }

        if(icon != null) {
            float iconY = transform.parent.eulerAngles.y;
            float iconX = icon.transform.eulerAngles.x;
            float iconZ = icon.transform.eulerAngles.z;
            icon.transform.rotation = Quaternion.Euler(90f, iconY, 0f);

        }
    }  */ 
    
        
}
