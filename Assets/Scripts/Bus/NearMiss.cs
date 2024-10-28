using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NearMiss : MonoBehaviour
{
    [SerializeField] private GameObject bus; // Reference to the bus
    [SerializeField] private GameObject comboObject;
    [SerializeField] private TextMeshProUGUI comboUI;
    public List<GameObject> InsideTriggerBox = new List<GameObject>();
    private int Combo = 0;

    private void Start()
    {
        Combo = 0;
        comboObject.SetActive(false);
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
        if (other.gameObject != bus)
        {
            if (InsideTriggerBox.Contains(other.gameObject))
            {
                ++Combo;
                comboObject.SetActive(true);
                comboUI.text = "NEAR MISS COMBO: " + Combo;
                InsideTriggerBox.Remove(other.gameObject);
            }
        }
    }

    public void BusCollisionWith()
    {
        Combo = 0;
        comboObject.SetActive(false);
        InsideTriggerBox.Clear();
    }
}
