using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeVehicleController;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private float m_CollisionCooldown;
    private CameraShake m_CamShake;
    private GameObject bus;
    private ChaosObjectiveHandler objectiveHandler;
    [SerializeField] GameObject sparks;

    [Header("Light Obstacle Camera Shake")]
    [SerializeField] private float minSpeedLight = 10f;
    [SerializeField] private float m_LightDuration;
    [Range(0.00f, 1.00f)] [SerializeField] private float m_LightIntensity;

    [Header("Medium Obstacle Camera Shake")]
    [SerializeField] private float minSpeedMedium = 15f;
    [SerializeField] private float m_MediumDuration;
    [Range(0.00f, 1.00f)] [SerializeField] private float m_MediumIntensity;

    [Header("Heavy Obstacle Camera Shake")]
    [SerializeField] private float minSpeedHeavy = 40f;
    [SerializeField] private float m_HeavyDuration;
    [Range(0.00f, 1.00f)] [SerializeField] private float m_HeavyIntensity;

    [Header("Lost Ejection Settings")]
    [Range(0f, 1f)] [SerializeField] private float lostChance = 0.5f;
    [SerializeField] private float minCrashSpeed = 65f;
    [SerializeField] private float EjectionForce = 200f;
    [SerializeField] private float verticaDirection = 1.5f;

    [SerializeField] private NearMiss nearMiss;
    BusAudioHandler audioHandler;

    public enum CrashTypes
    {
        None,
        Light,
        Medium,
        Heavy
    }

    private bool m_CanCollide = true;
    private float m_CurrentTime;
    // Start is called before the first frame update
    void Start()
    {
        audioHandler = GetComponent<BusAudioHandler>();
        m_CamShake = Camera.main.GetComponent<CameraShake>();

        if (m_CamShake == null)
            return;

        m_CanCollide = true;
        m_CurrentTime = 0;

        bus = FindAnyObjectByType<BombMeter>().gameObject;
        FindAnyObjectByType<BombMeter>().SetCrashSpeedUI(minCrashSpeed);
        objectiveHandler = gameObject.GetComponent<ChaosObjectiveHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_CanCollide)
        {
            if (m_CurrentTime > 0)
            {
                m_CurrentTime -= Time.deltaTime;
            }

            else
            {
                m_CurrentTime = m_CollisionCooldown;
                m_CanCollide = true;
            }
        }
    }

    public void ExecuteCollisionShit(ObstacleTag obstacleType)
    {
        switch (obstacleType)
        {
            case ObstacleTag.None:
                break;
            case ObstacleTag.Pedestrian:
                m_CamShake.DoCameraShake(m_LightIntensity, m_LightDuration);
                break;
            case ObstacleTag.CarAI:
                m_CamShake.DoCameraShake(m_HeavyIntensity, m_HeavyDuration);
                break;
            case ObstacleTag.Light:
                m_CamShake.DoCameraShake(m_LightIntensity, m_LightDuration);
                break;
            case ObstacleTag.Medium:
                m_CamShake.DoCameraShake(m_MediumIntensity, m_MediumDuration);
                break;
            case ObstacleTag.Heavy:
                m_CamShake.DoCameraShake(m_HeavyIntensity, m_HeavyDuration);
                break;
            default:
                m_CamShake.DoCameraShake(m_LightIntensity, m_LightDuration);
                break;
        }
    }

    
    private CrashTypes CollisionManager(ObstacleTag obstacleType, float speed)
    {
        switch (obstacleType)
        {
            case ObstacleTag.Light:
                if (speed >= minSpeedLight)
                    return CrashTypes.Light;
                break;

            case ObstacleTag.Medium:
            case ObstacleTag.Pedestrian:
                if (speed >= minSpeedMedium)
                    return CrashTypes.Medium;
                else if (speed >= minSpeedLight)
                    return CrashTypes.Light;
                break;

            case ObstacleTag.CarAI:
            case ObstacleTag.Heavy:
                if (speed >= minSpeedHeavy)
                    return CrashTypes.Heavy;
                else if (speed >= minSpeedMedium)
                    return CrashTypes.Medium;
                else if(speed >= minSpeedLight)
                    return CrashTypes.Light;
                break;

            case ObstacleTag.None:
            default:
                return CrashTypes.None;
        }

        return CrashTypes.None; // Default return if no conditions are met
    }
    

    private bool CheckCrash(ObstacleTag obstacleType, float speed)
    {
        switch (obstacleType)
        {
            case ObstacleTag.Medium:
            case ObstacleTag.CarAI:
            case ObstacleTag.Heavy:
                if (speed >= minCrashSpeed)
                    return true;
                else
                    return false;

            case ObstacleTag.Light:
            case ObstacleTag.Pedestrian:
            case ObstacleTag.None:
            default:
                return false;
        }
    }

    private AudioClip CrashSound(ObstacleTag obstacleType)
    {
        switch (obstacleType)
        {
            case ObstacleTag.Medium:
                return audioHandler.mCrash;
            case ObstacleTag.CarAI:
                return audioHandler.mCrash;
            case ObstacleTag.Heavy:
                return audioHandler.lCrash;
            case ObstacleTag.Light:
                int random = Random.Range(0, audioHandler.sCrash.Length);
                return audioHandler.sCrash[random];
            case ObstacleTag.Pedestrian:
            default:
                int random2 = Random.Range(0, audioHandler.sCrash.Length);
                return audioHandler.sCrash[random2];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float crashImpact = (collision.relativeVelocity).magnitude;
        if (m_CanCollide && crashImpact > 5f && (collision.gameObject.GetComponent<ObstacleType>() == null || (collision.gameObject.GetComponent<ObstacleType>() != null && collision.gameObject.GetComponent<ObstacleType>().obstacleTag != ObstacleTag.Light)))
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;


            Instantiate(sparks, pos, rot);
            audioHandler.PlayOneShotSFX(CrashSound(ObstacleTag.None));
            m_CamShake.DoCameraShake(m_LightIntensity, m_LightDuration);
        }


        ObstacleType obs = collision.gameObject.GetComponent<ObstacleType>();

        if (obs == null) return;

        ObstacleTag m_ObstacleType = obs.obstacleTag;

        if(collision.gameObject != null)
        {
            if (objectiveHandler.active && objectiveHandler.chaosType == ChaosType.collision)
            {
                objectiveHandler.requirement--;
            }
        }

        if (m_CanCollide)
        {
            m_CanCollide = false;

            if(m_ObstacleType == ObstacleTag.CarAI)
            {
                if (objectiveHandler.active && objectiveHandler.chaosType == ChaosType.carCrash)
                {
                    objectiveHandler.requirement--;
                }
            }

            audioHandler.PlayOneShotSFX(CrashSound(m_ObstacleType));

            ExecuteCollisionShit(obs.obstacleTag);

            // Get the relative velocity of the collision, which can be used to calculate the force and direction
            Vector3 crashDirection = (-collision.relativeVelocity).normalized; // Direction of the crash
            crashDirection += new Vector3(0, verticaDirection, 0);

            // Call the CrashHandler function with the calculated force and direction
            if (CheckCrash(m_ObstacleType, bus.GetComponent<BombMeter>().GetCurrentSpeed()))
            {
                if (Random.Range(0f, 1f) <= lostChance)
                {
                    bus.GetComponent<BusPassengers>().CrashEjectPassenger(crashDirection, EjectionForce);
                }
            }

            // Trigger NearMiss behavior
            nearMiss.BusCollisionWith();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        float crashImpact = (collision.relativeVelocity).magnitude;

        if (m_CanCollide && crashImpact > 20f && collision.gameObject.GetComponent<ObstacleType>() == null)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            Instantiate(sparks, pos, rot);
        }
    }

}
