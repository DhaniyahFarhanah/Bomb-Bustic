using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampIcon : MonoBehaviour
{

    public Camera mapCamera;
    private Transform mapCam;
    public float mapSize = 180f;
    Vector3 temp;
    // Start is called before the first frame update
    void Start()
    {
        mapCam = mapCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        temp = transform.parent.parent.position;
        temp.y = transform.position.y;
        transform.position = temp;
    }

    void LateUpdate() {
        transform.position = new Vector3 (
            Mathf.Clamp(transform.position.x, mapCam.position.x -mapSize, mapSize + mapCam.position.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, mapCam.position.z - mapSize, mapSize + mapCam.position.z)
        );

    }
}
