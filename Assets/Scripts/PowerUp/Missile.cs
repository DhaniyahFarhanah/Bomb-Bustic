using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//So the missile will move at the cursor position but will home into the first target it sees

public class Missile : MonoBehaviour
{
    private float force = 100f;
    private float speed = 5f;
    private Rigidbody rb;
    public GameObject target;
    //public Vector3 Crosshair;
    public bool homeIn;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * force;
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (homeIn)
        {
            rb.velocity = Vector3.zero;
            speed += Time.deltaTime * 100f;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime * 5f);
        }
        /*else
        {
            speed += Time.deltaTime * 100f;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime * 5f);
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        if (hitObject.GetComponentInParent<RougeAI>())
        {
            hitObject.GetComponentInParent<RougeAI>().SelfDestruct();
            Destroy(gameObject);
            return;
        }

        if (hitObject.GetComponent<ObstacleType>() != null && !hitObject.CompareTag("Player") && !hitObject.CompareTag("Indestructable") && !hitObject.CompareTag("DropOff"))
        {
            Destroy(hitObject.transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}
