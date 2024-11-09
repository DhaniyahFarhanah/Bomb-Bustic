using System.Collections.Generic;
using UnityEngine;

public class BasicAI : AICarEngine
{
    [Header("Basic AI")]
    public Path path;
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    public float waypointBuffer = 3f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypoint = 0;
    public enum AIState { DrivingNormal, AvoidingObstacle, StopVehicleAhead }
    public AIState State = AIState.DrivingNormal;

    [Header("Traffic")]
    public float stoppingDistance = 1f;
    public float decelerationDistance = 5f;

    private Transform cachedTransform;
    private float waypointBufferSqr;
    private float decelerationDistanceSqr;
    private float stoppingDistanceSqr;
    private float highSpeed;
    private float sharpTurn;  

    // Start is called before the first frame update
    private void Start()
    {
        base.Init();
        waypoints = path.waypoints;

        cachedTransform = transform;
        waypointBufferSqr = waypointBuffer * waypointBuffer;
        highSpeed = maxSpeed * highSpeedThreshold;
        sharpTurn = maxSteerAngle * sharpTurnThreshold;
        decelerationDistanceSqr = decelerationDistance * decelerationDistance;
        stoppingDistanceSqr = stoppingDistance * stoppingDistance;

        FindNearestNode();
    }

    private void FixedUpdate()
    {
        EngineUpdate();
        CheckWaypointDistance();  // Check if the car is near the current waypoint
    }

    #region Route
    private void FindNearestNode()
    {
        float nearestDistance = Mathf.Infinity;  // Set an initially large value for comparison
        int nearestNodeIndex = 0;  // Variable to store the index of the nearest node
        Vector3 currentPos = cachedTransform.position;

        // Loop through all nodes
        for (int i = 0; i < waypoints.Count; i++)
        {
            // Calculate the distance between the car and the current node
            float distanceSqr = (currentPos - waypoints[i].position).sqrMagnitude;

            // If the current node is closer than the previously found nearest node
            if (distanceSqr < nearestDistance)
            {
                nearestDistance = distanceSqr;  // Update the nearest distance
                nearestNodeIndex = i;  // Update the nearest node index
            }
        }

        // Set the nearest node as the current node the car should travel to
        currentWaypoint = nearestNodeIndex;
        targetPosition = waypoints[currentWaypoint].position;
    }

    private void CheckWaypointDistance()
    {
        Vector3 currentPos = cachedTransform.position;
        if ((currentPos - waypoints[currentWaypoint].position).sqrMagnitude < waypointBufferSqr)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;

            targetPosition = waypoints[currentWaypoint].position;
        }
    }
    #endregion Route

    protected override void ObstacleResponse()
    {
        if (!detectedObstacle)
        {
            State = AIState.DrivingNormal;
            return;
        }

        ObstacleType obstacleType = detectedObstacleHit.collider.gameObject.GetComponent<ObstacleType>();
        if (obstacleType == null) return;

        switch (obstacleType.obstacleTag)
        {
            case ObstacleTag.Light:
            case ObstacleTag.Medium:
            case ObstacleTag.Heavy:
            case ObstacleTag.Pedestrian:
                State = AIState.AvoidingObstacle;
                break;

            case ObstacleTag.CarAI:
            case ObstacleTag.Player:
                State = Vector3.Dot(cachedTransform.forward, detectedObstacleHit.collider.transform.forward) > 0f
                    ? AIState.StopVehicleAhead
                    : AIState.AvoidingObstacle;
                break;

            case ObstacleTag.None:
            default:
                State = AIState.DrivingNormal;
                break;
        }
    }

    protected override void BrakeLogic()
    {
        isBraking = false;

        if (State == AIState.AvoidingObstacle && slowWhenAvoiding && currentSpeed > highSpeed)
        {
            isBraking = true;
        }
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < stoppingDistance)
        {
            isBraking = true;
        }
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < decelerationDistance && currentSpeed > highSpeed)
        {
            isBraking = true;
        }
        else if (slowWhenTurning && Mathf.Abs(wheelFL.steerAngle) >= sharpTurn && currentSpeed > highSpeed)
        {
            isBraking = true;
        }
        Vector3 carForwardPos = cachedTransform.position + cachedTransform.forward * frontSensorPosition.z + cachedTransform.up * frontSensorPosition.y;
        float distanceToLightSqr = (carForwardPos - waypoints[currentWaypoint].position).sqrMagnitude;

        switch (waypoints[currentWaypoint].GetComponent<Waypoint>().GetState())
        {
            case Waypoint.State.YellowEarly:
                if (distanceToLightSqr < decelerationDistanceSqr && currentSpeed > highSpeed * 0.5f)
                {
                    isBraking = true;
                }
                break;
            case Waypoint.State.YellowLate:
            case Waypoint.State.Red:
                if (distanceToLightSqr < stoppingDistanceSqr ||
                    (distanceToLightSqr < decelerationDistanceSqr && currentSpeed > highSpeed * 0.5f))
                {
                    isBraking = true;
                }
                break;
            case Waypoint.State.Green:
            default:
                break;
        }
    }

    protected override void ApplySteer()
    {
        if (State == AIState.AvoidingObstacle)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
        else
        {
            if (debugLine)
                Debug.DrawLine(transform.position, targetPosition, targetLineColor);

            Vector3 relativeVector = cachedTransform.InverseTransformPoint(targetPosition);
            targetSteerAngle = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        }
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : AICarEngine
{
    [Header("Basic AI")]
    public Path path;
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    public float waypointBuffer = 3f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypoint = 0;
    public enum AIState
    {
        DrivingNormal,
        AvoidingObstacle,
        StopVehicleAhead
    }
    public AIState State = AIState.DrivingNormal;

    [Header("Traffic")]
    public float stoppingDistance = 1f;
    public float decelerationDistance = 5f;

    // Start is called before the first frame update
    private void Start()
    {
        base.Init();
        waypoints = path.waypoints;
        FindNearestNode();
    }

    private void FixedUpdate()
    {
        EngineUpdate();
        CheckWaypointDistance();  // Check if the car is near the current waypoint
    }

    #region Route
    private void FindNearestNode()
    {
        float nearestDistance = Mathf.Infinity;  // Set an initially large value for comparison
        int nearestNodeIndex = 0;  // Variable to store the index of the nearest node

        // Loop through all nodes
        for (int i = 0; i < waypoints.Count; i++)
        {
            // Calculate the distance between the car and the current node
            float distance = Vector3.Distance(transform.position, waypoints[i].position);

            // If the current node is closer than the previously found nearest node
            if (distance < nearestDistance)
            {
                nearestDistance = distance;  // Update the nearest distance
                nearestNodeIndex = i;  // Update the nearest node index
            }
        }

        // Set the nearest node as the current node the car should travel to
        currentWaypoint = nearestNodeIndex;
        targetPosition = waypoints[currentWaypoint].position;
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < waypointBuffer)
        {
            if (currentWaypoint == waypoints.Count - 1)
            {
                currentWaypoint = 0;  // Loop back to the first node when all waypoints are reached
            }
            else
            {
                ++currentWaypoint;  // Move to the next waypoint
            }
            targetPosition = waypoints[currentWaypoint].position;
        }
    }
    #endregion Route

    protected override void ObstacleResponse()
    {
        if (!detectedObstacle)
        {
            State = AIState.DrivingNormal;
        }
        else
        {
            ObstacleTag sensedObstacleTag = detectedObstacleHit.collider.gameObject.GetComponent<ObstacleType>().obstacleTag;

            if (sensedObstacleTag == ObstacleTag.Light || sensedObstacleTag == ObstacleTag.Medium || sensedObstacleTag == ObstacleTag.Heavy || sensedObstacleTag == ObstacleTag.Pedestrian)
            {
                State = AIState.AvoidingObstacle;
            }
            else if (sensedObstacleTag == ObstacleTag.CarAI || sensedObstacleTag == ObstacleTag.Player)
            {
                if (Vector3.Dot(transform.forward, detectedObstacleHit.collider.gameObject.transform.forward) > 0f)
                {
                    State = AIState.StopVehicleAhead;
                }
                else
                {
                    State = AIState.AvoidingObstacle;
                }
            }
            else if (sensedObstacleTag == ObstacleTag.None)
            {
                State = AIState.DrivingNormal;
            }
        }
    }

    protected override void BrakeLogic()
    {
        isBraking = false;
        if (slowWhenAvoiding && State == AIState.AvoidingObstacle && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (slowWhenTurning && Mathf.Abs(wheelFL.steerAngle) >= maxSteerAngle * sharpTurnThreshold && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < stoppingDistance)
        {
            isBraking = true;
        }

        Vector3 carForwardPosition = transform.position;
        carForwardPosition += transform.forward * frontSensorPosition.z;
        carForwardPosition += transform.up * frontSensorPosition.y;
        float distanceToLight = Vector3.Distance(carForwardPosition, waypoints[currentWaypoint].position);

        switch (waypoints[currentWaypoint].GetComponent<Waypoint>().GetState())
        {
            case Waypoint.State.Green:
                break;
            case Waypoint.State.YellowEarly:
                if (distanceToLight < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold * 0.5f)
                {
                    isBraking = true;
                }
                break;
            case Waypoint.State.YellowLate:
            case Waypoint.State.Red:
                if (distanceToLight < stoppingDistance)
                {
                    isBraking = true;
                }
                else if (distanceToLight < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold * 0.5f)
                {
                    isBraking = true;
                }
                break;
        }
    }

    protected override void ApplySteer()
    {
        if (State == AIState.AvoidingObstacle)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
        else
        {
            if (debugLine)
                Debug.DrawLine(transform.position, targetPosition, targetLineColor);

            Vector3 relativeVector = transform.InverseTransformPoint(targetPosition);
            float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            targetSteerAngle = newSteer;
        }
    }
}
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : AICarEngine
{
    [Header("Basic AI")]
    public Path path;
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    public float waypointBuffer = 3f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypoint = 0;
    public enum AIState
    {
        DrivingNormal,
        AvoidingObstacle,
        StopVehicleAhead
    }
    public AIState State = AIState.DrivingNormal;

    [Header("Traffic")]
    public float stoppingDistance = 1f;
    public float decelerationDistance = 5f;

    // Start is called before the first frame update
    private void Start()
    {
        base.Init();
        waypoints = path.waypoints;
        FindNearestNode();
    }

    private void FixedUpdate()
    {
        EngineUpdate();
        CheckWaypointDistance();  // Check if the car is near the current waypoint
    }

    #region Route
    private void FindNearestNode()
    {
        float nearestDistance = Mathf.Infinity;  // Set an initially large value for comparison
        int nearestNodeIndex = 0;  // Variable to store the index of the nearest node

        // Loop through all nodes
        for (int i = 0; i < waypoints.Count; i++)
        {
            // Calculate the distance between the car and the current node
            float distance = Vector3.Distance(transform.position, waypoints[i].position);

            // If the current node is closer than the previously found nearest node
            if (distance < nearestDistance)
            {
                nearestDistance = distance;  // Update the nearest distance
                nearestNodeIndex = i;  // Update the nearest node index
            }
        }

        // Set the nearest node as the current node the car should travel to
        currentWaypoint = nearestNodeIndex;
        targetPosition = waypoints[currentWaypoint].position;
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < waypointBuffer)
        {
            if (currentWaypoint == waypoints.Count - 1)
            {
                currentWaypoint = 0;  // Loop back to the first node when all waypoints are reached
            }
            else
            {
                ++currentWaypoint;  // Move to the next waypoint
            }
            targetPosition = waypoints[currentWaypoint].position;
        }
    }
    #endregion Route

    protected override void ObstacleResponse()
    {
        if (!detectedObstacle)
        {
            State = AIState.DrivingNormal;
        }
        else
        {
            ObstacleTag sensedObstacleTag = detectedObstacleHit.collider.gameObject.GetComponent<ObstacleType>().obstacleTag;

            if (sensedObstacleTag == ObstacleTag.Light || sensedObstacleTag == ObstacleTag.Medium || sensedObstacleTag == ObstacleTag.Heavy || sensedObstacleTag == ObstacleTag.Pedestrian)
            {
                State = AIState.AvoidingObstacle;
            }
            else if (sensedObstacleTag == ObstacleTag.CarAI || sensedObstacleTag == ObstacleTag.Player)
            {
                if (Vector3.Dot(transform.forward, detectedObstacleHit.collider.gameObject.transform.forward) > 0f)
                {
                    State = AIState.StopVehicleAhead;
                }
                else
                {
                    State = AIState.AvoidingObstacle;
                }
            }
            else if (sensedObstacleTag == ObstacleTag.None)
            {
                State = AIState.DrivingNormal;
            }
        }
    }

    protected override void BrakeLogic()
    {
        isBraking = false;
        if (slowWhenAvoiding && State == AIState.AvoidingObstacle && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (slowWhenTurning && Mathf.Abs(wheelFL.steerAngle) >= maxSteerAngle * sharpTurnThreshold && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < stoppingDistance)
        {
            isBraking = true;
        }

        Vector3 carForwardPosition = transform.position;
        carForwardPosition += transform.forward * frontSensorPosition.z;
        carForwardPosition += transform.up * frontSensorPosition.y;
        float distanceToLight = Vector3.Distance(carForwardPosition, waypoints[currentWaypoint].position);

        switch (waypoints[currentWaypoint].GetComponent<Waypoint>().GetState())
        {
            case Waypoint.State.Green:
                break;
            case Waypoint.State.YellowEarly:
                if (distanceToLight < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold * 0.5f)
                {
                    isBraking = true;
                }
                break;
            case Waypoint.State.YellowLate:
            case Waypoint.State.Red:
                if (distanceToLight < stoppingDistance)
                {
                    isBraking = true;
                }
                else if (distanceToLight < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold * 0.5f)
                {
                    isBraking = true;
                }
                break;
        }
    }

    protected override void ApplySteer()
    {
        if (State == AIState.AvoidingObstacle)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
        else
        {
            if (debugLine)
                Debug.DrawLine(transform.position, targetPosition, targetLineColor);

            Vector3 relativeVector = transform.InverseTransformPoint(targetPosition);
            float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            targetSteerAngle = newSteer;
        }
    }
}
*/