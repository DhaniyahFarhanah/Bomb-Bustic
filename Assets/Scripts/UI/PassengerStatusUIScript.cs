using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerStatusUIScript : MonoBehaviour
{
    [SerializeField] UIManager manager;
    [SerializeField] bool win;
    [SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        if (win)
        {
            manager.StartCoroutine(manager.InstantiatePassengers(time));
        }

        else
        {
            manager.StartCoroutine(manager.InstantiatePassengersLose(time));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
