using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] UIManager manager;
    [SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        manager.StartCoroutine(manager.InstantiatePassengers(time));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
