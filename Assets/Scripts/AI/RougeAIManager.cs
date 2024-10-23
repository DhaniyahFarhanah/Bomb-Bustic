using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeAIManager : MonoBehaviour
{
    [SerializeField] private GameObject rougeAIPrefab;
    [SerializeField] private List<GameObject> SpawnPoints;
    [SerializeField] private float chaseDelay = 60f;
    [SerializeField] private float chaseDuration = 60f;
    [SerializeField] private float minAI = 8;
    [SerializeField] NodeGraph nodeGraph;
    private enum RougeAIStates
    {
        CountDownStart,
        CountingDown,
        ChaseStart,
        ChaseActive,
        ChaseEnd,
    }
    RougeAIStates rougeAIState = RougeAIStates.CountDownStart;
    private float elaspedTime;
    private List<RougeAI> rougeAIs;


    // Start is called before the first frame update
    void Start()
    {
        elaspedTime = chaseDelay;
        rougeAIs = new List<RougeAI>(FindObjectsOfType<RougeAI>());
    }

    // Update is called once per frame
    void Update()
    {
        switch (rougeAIState)
        {
            case RougeAIStates.CountDownStart:
            {
                elaspedTime = chaseDelay;
                rougeAIState = RougeAIStates.CountingDown;
                break;
            }
            case RougeAIStates.CountingDown:
            {
                if (elaspedTime > 0f)
                {
                    elaspedTime -= Time.deltaTime;
                }
                else
                {
                    rougeAIState = RougeAIStates.ChaseStart;
                }
                break;
            }
            case RougeAIStates.ChaseStart:
            {
                FindAnyObjectByType<PoliceUI>().activatePoliceUI();
                rougeAIState = RougeAIStates.ChaseStart;
                foreach (RougeAI ai in rougeAIs)
                {
                    StartCoroutine(ai.ActiveAI());
                }
                elaspedTime = chaseDuration;
                rougeAIState = RougeAIStates.ChaseActive;
                break;
            }
            case RougeAIStates.ChaseActive:
            {
                if (elaspedTime > 0f)
                {
                    elaspedTime -= Time.deltaTime;
                }
                else
                {
                    rougeAIState = RougeAIStates.ChaseEnd;
                }

                if (rougeAIs.Count < minAI)
                {
                    GameObject newAI = Instantiate(rougeAIPrefab, SpawnPoints[0].transform.position, Quaternion.identity, gameObject.transform);
                    newAI.GetComponent<RougeAI>().nodeGraph = nodeGraph;
                    StartCoroutine(newAI.GetComponent<RougeAI>().ActiveAI());
                    rougeAIs.Add(newAI.GetComponent<RougeAI>());
                }
                break;
            }
            case RougeAIStates.ChaseEnd:
            {
                foreach (RougeAI ai in rougeAIs)
                {
                    Destroy(ai.gameObject);
                }
                rougeAIState = RougeAIStates.CountDownStart;
                break;
            }
        }

        if (elaspedTime > 0f)
        {
            elaspedTime -= Time.deltaTime;
        }
        else
        {
            FindAnyObjectByType<PoliceUI>().activatePoliceUI();
            foreach (RougeAI ai in rougeAIs)
            {
                ai.ActiveAI();
            }
        }
    }
}
