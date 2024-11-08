using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NearMiss : MonoBehaviour
{
    [SerializeField] private GameObject bus; // Reference to the bus
    [SerializeField] private ChaosObjectiveHandler objectiveHandler;
    public List<GameObject> InsideTriggerBox = new List<GameObject>();
    private int Combo = 0;

    private void Start()
    {
        Combo = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        ObstacleType obs = other.gameObject.GetComponent<ObstacleType>();
        if (obs == null) return;

        // Ensure the object entering the trigger is not the bus itself
        if (other.gameObject != bus)
        {
            //Debug.Log($"Added {other.gameObject.name}");
            InsideTriggerBox.Add(other.gameObject.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ObstacleType obs = other.gameObject.GetComponent<ObstacleType>();
        if (obs == null) return;

        // Ensure the object exiting the trigger is not the bus itself
        if (other.gameObject != bus && objectiveHandler.active && objectiveHandler.chaosType == ChaosType.miss)
        {
            Combo++;
            if (InsideTriggerBox.Contains(other.gameObject))
            {
                objectiveHandler.requirement--;
                InsideTriggerBox.Remove(other.gameObject);
            }
        }
    }

    public void BusCollisionWith()
    {
        Combo = 0;
        InsideTriggerBox.Clear();
    }
}
