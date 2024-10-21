using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private float speed;
    private Rigidbody rb;
    public Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.velocity = transform.forward * speed;
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        speed += Time.deltaTime * 100f;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ObstacleType>() != null && !other.gameObject.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Spawn explosion
        if(collision.gameObject.GetComponent<ObstacleType>() != null && !collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }


    }
}
