using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public LayerMask miniMask;
    public GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update() {
         GameObject target = GameObject.FindGameObjectWithTag("Player");
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Quaternion targetRot = target.transform.rotation;
        //transform.rotation = Quaternion.Euler(90f, targetRot.eulerAngles.y, targetRot.eulerAngles.z);
        icon.transform.rotation = Quaternion.Euler(icon.transform.eulerAngles.x , icon.transform.eulerAngles.y -90f, icon.transform.eulerAngles.y);
    }
    
    void LateUpdate() {

    }
    
        
}
