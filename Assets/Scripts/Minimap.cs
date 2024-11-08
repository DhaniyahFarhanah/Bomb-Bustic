using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public LayerMask miniMask;
    public GameObject icon;
    public float iconOffset = 25f;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {    
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Quaternion targetRot = target.transform.rotation;
        icon.transform.position = new Vector3(transform.position.x, 500.0f, transform.position.z);
        icon.transform.rotation = Quaternion.Euler(icon.transform.eulerAngles.x , transform.eulerAngles.y -90f, targetRot.eulerAngles.y);
    }
    
    void LateUpdate() {

    }
    
        
}
