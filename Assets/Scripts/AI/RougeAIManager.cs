using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeAIManager : MonoBehaviour
{
    [SerializeField] private GameObject rougeAIPrefab;
    [SerializeField] private List<GameObject> SpawnPoints;
    [SerializeField] private NodeGraph nodeGraph;
    [SerializeField] private float chaseDelay = 60f;
    [SerializeField] private float chaseDuration = 60f;
    [SerializeField] private float minAI = 8f;
    [SerializeField] private float spawnInterval = 15f;

    private enum RougeAIStates
    {
        CountDownStart,
        CountingDown,
        ChaseStart,
        ChaseActive,
        ChaseEnd,
    }

    private RougeAIStates rougeAIState = RougeAIStates.CountDownStart;
    private float elapsedTime;
    private float spawnTimer;
    private List<RougeAI> rougeAIs;
    private GameObject player;

    void Start()
    {
        elapsedTime = chaseDelay;
        spawnTimer = spawnInterval; // Initialize spawn timer
        rougeAIs = new List<RougeAI>(FindObjectsOfType<RougeAI>());
        player = GameObject.FindGameObjectWithTag("Player"); // Find player
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && rougeAIState != RougeAIStates.ChaseActive)
        {
            rougeAIState = RougeAIStates.ChaseStart;
        }

        UpdateState();
        ManageAISpawning();
    }

    private void UpdateState()
    {
        switch (rougeAIState)
        {
            case RougeAIStates.CountDownStart:
                StartCountdown();
                break;

            case RougeAIStates.CountingDown:
                if (IsTimeUp())
                {
                    rougeAIState = RougeAIStates.ChaseStart;
                }
                break;

            case RougeAIStates.ChaseStart:
                StartChase();
                break;

            case RougeAIStates.ChaseActive:
                if (IsTimeUp())
                {
                    rougeAIState = RougeAIStates.ChaseEnd;
                }
                break;

            case RougeAIStates.ChaseEnd:
                EndChase();
                break;
        }
    }

    private void StartCountdown()
    {
        elapsedTime = chaseDelay;
        rougeAIState = RougeAIStates.CountingDown;
    }

    private void StartChase()
    {
        foreach (RougeAI ai in rougeAIs)
        {
            StartCoroutine(ai.ActiveAI());
        }
        elapsedTime = chaseDuration;
        rougeAIState = RougeAIStates.ChaseActive;
    }

    private void EndChase()
    {
        foreach (RougeAI ai in rougeAIs)
        {
            if(ai != null)
            {
                Destroy(ai.gameObject);
            }
            
        }

        rougeAIs.Clear();  // Clear list after destroying AIs
        rougeAIState = RougeAIStates.CountDownStart;
    }

    private void ManageAISpawning()
    {
        // Reduce spawn timer only when in ChaseActive state
        if (rougeAIState == RougeAIStates.ChaseActive)
        {
            spawnTimer -= Time.deltaTime;

            if (rougeAIs.Count < minAI && spawnTimer <= 0f)
            {
                // Find the farthest spawn point from the player
                GameObject farthestSpawnPoint = FindFarthestSpawnPoint();

                // Instantiate a new AI at the farthest spawn point
                GameObject newAI = Instantiate(rougeAIPrefab, farthestSpawnPoint.transform.position, Quaternion.identity, gameObject.transform);
                RougeAI aiComponent = newAI.GetComponent<RougeAI>();
                aiComponent.nodeGraph = nodeGraph;
                StartCoroutine(aiComponent.ActiveAI());
                rougeAIs.Add(aiComponent);

                // Reset the spawn timer
                spawnTimer = spawnInterval;
            }
        }
    }

    private GameObject FindFarthestSpawnPoint()
    {
        GameObject farthestSpawnPoint = null;
        float maxDistance = float.MinValue;

        foreach (GameObject spawnPoint in SpawnPoints)
        {
            float distance = Vector3.Distance(spawnPoint.transform.position, player.transform.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestSpawnPoint = spawnPoint;
            }
        }

        return farthestSpawnPoint;
    }

    private bool IsTimeUp()
    {
        elapsedTime -= Time.deltaTime;
        return elapsedTime <= 0f;
    }
}
