using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class IconRotationClamp : MonoBehaviour
{
    public Transform ActualObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate() {
        transform.rotation = Quaternion.Euler(90.0f, ActualObject.eulerAngles.y, 0.0f);
    }
}
