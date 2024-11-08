using ArcadeVehicleController;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum ChaosType
{
    backwards,
    carCrash,
    collision,
    powerUp,
    miss
}

public class ChaosObjectiveHandler : MonoBehaviour
{
    [Header("Instantiating")]
    [SerializeField] private GameObject objectiveShowcase;
    [SerializeField] private Animator objectShowcaseAnim;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text requirementText;
    [SerializeField] private TMP_Text rewardAmtText;
    [SerializeField] private Image timerTime;
    private ScaledBombSystem bombSystem;
    private Vehicle bus;
    private JeepVisual busVisual;
    private PowerUpHandler powerUpHandler;

    [Header("Objectives")]
    public ChaosType chaosType;
    public bool active;
    [SerializeField] private float duration;
    [SerializeField] private float maxSecsTilNew;
    [SerializeField] private float minSecsTilNew;
    [HideInInspector] public int reward;
    private string objectiveString;
    public float requirement;

    [SerializeField] private float newTimer;
    [SerializeField] private float randomTimer;
    [SerializeField] private float durationTimer;

    [Header("Backwards Objective")]
    [SerializeField] private string backwardsObjectiveText;
    [SerializeField] private float amtOfSeconds;
    [SerializeField] private int backwardReward;

    [Header("Car Crash Objective")]
    [SerializeField] private string carCrashObjectiveText;
    [SerializeField] private int numOfCars;
    [SerializeField] private int carCrashReward;

    [Header("Collision Objective")]
    [SerializeField] private string collisionObjectiveText;
    [SerializeField] private int numTimesToCollide;
    [SerializeField] private int collideReward;

    [Header("Power Up")]
    [SerializeField] private string pWObjectiveText;
    [SerializeField] private int numToUse;
    [SerializeField] private int pWReward;

    [Header("Miss")]
    [SerializeField] private string nearMissObjectiveText;
    [SerializeField] private int nearMiss;
    [SerializeField] private int nearMissReward;


    // Start is called before the first frame update
    void Start()
    {
        randomTimer = Random.Range(minSecsTilNew, maxSecsTilNew);
        newTimer = randomTimer;

        bombSystem = gameObject.GetComponent<ScaledBombSystem>();
        bus = gameObject.GetComponent<Vehicle>();
        busVisual = gameObject.GetComponent<JeepVisual>();
        powerUpHandler = gameObject.GetComponent<PowerUpHandler>();

        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            if (newTimer > 0)
            {
                newTimer -= Time.deltaTime; 
            }
            else if (newTimer <= 0)
            {
                randomTimer = Random.Range(minSecsTilNew, maxSecsTilNew);
                newTimer = randomTimer;
                active = true;
                GiveObjective();
            }
        }

        else if(active)
        {
            if (durationTimer > 0)
            {
                durationTimer -= Time.deltaTime;
                timerTime.fillAmount = (durationTimer / duration);
                CheckObjective();
            }
            else if (durationTimer <= 0)
            {
                StartCoroutine(EndObjective(false));
            }
        }
    }

    void GiveObjective()
    {
        objectShowcaseAnim.SetBool("Done", false);
        durationTimer = duration;
        objectiveShowcase.SetActive(true);

        chaosType = (ChaosType)Random.Range(0, 5);
        requirement = 0;

        InstantiateObjective();
    }

    void InstantiateObjective()
    {
        switch (chaosType)
        {
            case ChaosType.powerUp:
                reward = pWReward;
                requirement = numToUse;
                rewardAmtText.text = pWReward.ToString() + "s";
                objectiveText.text = pWObjectiveText;
                break;

            case ChaosType.carCrash:
                reward = carCrashReward;
                requirement = numOfCars;
                rewardAmtText.text = "+" + carCrashReward.ToString() + "s";
                objectiveText.text = carCrashObjectiveText;
                break;

            case ChaosType.collision:
                reward = collideReward;
                requirement = numTimesToCollide;
                rewardAmtText.text = "+" + collideReward.ToString() + "s";
                objectiveText.text = collisionObjectiveText;
                break;

            case ChaosType.backwards: 
                reward = backwardReward;
                requirement = amtOfSeconds;
                rewardAmtText.text = "+" + backwardReward.ToString() + "s";
                objectiveText.text = backwardsObjectiveText;

                break;

            case ChaosType.miss: 
                reward = nearMissReward;
                requirement = nearMiss;
                rewardAmtText.text = "+" + nearMissReward.ToString() + "s";
                objectiveText.text = nearMissObjectiveText;
                break;
        }
    }

    void CheckObjective()
    {
        switch (chaosType)
        {
            case ChaosType.powerUp:
                requirementText.text = "x " + ((int)requirement).ToString() + "Time(s)";

                if (requirement <= 0)
                {
                    requirementText.text = "Success!";
                    StartCoroutine(EndObjective(true));
                }

                break;

            case ChaosType.carCrash:
                requirementText.text = "x " + ((int)requirement).ToString();

                if (requirement <= 0)
                {
                    requirementText.text = "Success!";
                    StartCoroutine(EndObjective(true));
                }

                break;

            case ChaosType.collision:
                requirementText.text = "x " + ((int)requirement).ToString();

                if (requirement <= 0)
                {
                    requirementText.text = "Success!";
                    StartCoroutine(EndObjective(true));
                }
                break;

            case ChaosType.backwards:
                requirementText.text = "for " + ((int)requirement).ToString() + "secs";

                if (busVisual.ForwardSpeed < -5.0f)
                {
                    requirement -= Time.deltaTime;
                }

                else if (requirement <= 0)
                {
                    requirementText.text = "Success!";
                    StartCoroutine(EndObjective(true));
                }
                else
                {
                    requirement = amtOfSeconds;
                }

                break;

            case ChaosType.miss:
                requirementText.text = "x " + ((int)requirement).ToString();

                if(requirement <= 0)
                {
                    requirementText.text = "Success!";
                    StartCoroutine(EndObjective(true));
                }
                break;
        }
        
    }

    IEnumerator EndObjective(bool success)
    {
        active = false;
        bombSystem.objectiveFinished = true;
        objectShowcaseAnim.SetBool("Done", true);
        //for as long as animation clip
        yield return new WaitForSeconds(1f);

        if (success)
        {
            bombSystem.AddTime(reward);
        }
        else if (!success)
        {
            bombSystem.AddTime(-10);
        }
        objectiveShowcase.SetActive(false);
    }



}
