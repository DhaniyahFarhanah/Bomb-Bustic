using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerup : MonoBehaviour
{
    [SerializeField] GameObject powerUpPrefab;
    [SerializeField] float spawnTimer;
    GameObject powerUp;
    float currentTimer;

    // Start is called before the first frame update
    void Start()
    {
        powerUp = Instantiate(powerUpPrefab, transform);
        currentTimer = spawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(powerUp == null)
        {
            //start timer
            if(currentTimer <= 0)
            {
                currentTimer = spawnTimer;
                powerUp = Instantiate(powerUpPrefab, transform);
            }

            if(currentTimer >= 0)
            {
                currentTimer -= Time.deltaTime;
            }
        }
    }
}
