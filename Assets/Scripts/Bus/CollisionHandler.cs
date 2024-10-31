using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeVehicleController;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private float m_CollisionCooldown;
    private CameraShake m_CamShake;
    private GameObject bus;

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
        m_CamShake = Camera.main.GetComponent<CameraShake>();

        if (m_CamShake == null)
            return;

        m_CanCollide = true;
        m_CurrentTime = 0;

        bus = FindAnyObjectByType<BombMeter>().gameObject;
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

    private void OnCollisionEnter(Collision collision)
    {
        ObstacleType obs = collision.gameObject.GetComponent<ObstacleType>();

        if (obs == null) return;

        ObstacleTag m_ObstacleType = obs.obstacleTag;
        if (m_CanCollide)
        {
            m_CanCollide = false;

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

}
