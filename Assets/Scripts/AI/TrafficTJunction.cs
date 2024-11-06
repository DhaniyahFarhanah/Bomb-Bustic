using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficTJunction : MonoBehaviour
{
    // List of traffic light objects to change color
    public float greenDuration = 10f;  // Duration for green light
    public float yellowDuration = 3f;  // Duration for yellow light
    [Range(0f, 1f)]
    public float yellowEarlyThreshold = 0.6f;
    public float redDuration = 10f;  // Duration for red light
    public float allRedDuration = 2f;  // Duration for all-red phase

    [Header("Main Road 1")]
    public List<Waypoint> lane1Waypoints;

    [Header("Main Road 2")]
    public List<Waypoint> lane2Waypoints;

    [Header("Main Road 3")]
    public List<Waypoint> lane3Waypoints;

    private void Start()
    {
        // Start the coroutine to control traffic light colors
        StartCoroutine(ChangeTrafficLightColors());
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = lineColor;
    //    Gizmos.DrawLine(Lane1Waypoint.transform.position, Lane2Waypoint.transform.position);
    //}

    // Coroutine to change traffic light colors
    IEnumerator ChangeTrafficLightColors()
    {
        while (true)
        {
            // Lane 1: Green, Lane 2,3: Red
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Green);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Yellow Early, Lane 2,3: Red
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowEarly);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1: Yellow Late, Lane 2,3: Red
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowLate);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }

            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(allRedDuration);

            // Lane 1,3: Red, Lane 2: Green
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Green);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(greenDuration);

            // Lane 1,3: Red, Lane 2: Yellow Early
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowEarly);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1,3: Red, Lane 2: Yellow Late
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowLate);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(allRedDuration);

            // Lane 1,2: Red, Lane 3: Green
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Green);  // Green for Lane 1
            }
            yield return new WaitForSeconds(greenDuration);

            // Lane 1,2: Red, Lane 3: Yellow Early
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowEarly);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1,2: Red, Lane 3: Yellow Late
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowLate);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane3Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(allRedDuration);
        }
    }

    // Helper function to set colors of traffic lights in a lane
    void SetLaneState(Waypoint node, Waypoint.State state)
    {
        if (node != null)
        {
            node.SetState(state);
        }
    }
}

//    private void Start()
//    {
//        // Start the coroutine to control traffic light colors
//        StartCoroutine(ChangeTrafficLightColors());
//    }

//    // Coroutine to change traffic light colors
//    IEnumerator ChangeTrafficLightColors()
//    {
//        while (true)
//        {
//            // Phase 1: Lane 1 Green, Lane 2 Red, Lane 3 Red
//            SetLaneState(lane1Waypoints, Waypoint.State.Green);
//            SetLaneState(lane2Waypoints, Waypoint.State.Red);
//            SetLaneState(lane3Waypoints, Waypoint.State.Red);
//            yield return new WaitForSeconds(greenDuration);

//            // Phase 2: Lane 1 Yellow Early, Lane 2 Red, Lane 3 Red
//            SetLaneState(lane1Waypoints, Waypoint.State.YellowEarly);
//            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

//            // Phase 3: Lane 1 Yellow Late, Lane 2 Red, Lane 3 Red
//            SetLaneState(lane1Waypoints, Waypoint.State.YellowLate);
//            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

//            // Phase 4: All lanes Red (All-red phase)
//            SetLaneState(lane1Waypoints, Waypoint.State.Red);
//            SetLaneState(lane2Waypoints, Waypoint.State.Red);
//            SetLaneState(lane3Waypoints, Waypoint.State.Red);
//            yield return new WaitForSeconds(allRedDuration);

//            // Phase 5: Lane 1 Red, Lane 2 Green, Lane 3 Green
//            SetLaneState(lane1Waypoints, Waypoint.State.Red);
//            SetLaneState(lane2Waypoints, Waypoint.State.Green);
//            SetLaneState(lane3Waypoints, Waypoint.State.Green);
//            yield return new WaitForSeconds(greenDuration);

//            // Phase 6: Lane 1 Red, Lane 2 Yellow Early, Lane 3 Yellow Early
//            SetLaneState(lane2Waypoints, Waypoint.State.YellowEarly);
//            SetLaneState(lane3Waypoints, Waypoint.State.YellowEarly);
//            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

//            // Phase 7: Lane 1 Red, Lane 2 Yellow Late, Lane 3 Yellow Late
//            SetLaneState(lane2Waypoints, Waypoint.State.YellowLate);
//            SetLaneState(lane3Waypoints, Waypoint.State.YellowLate);
//            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

//            // Phase 8: All lanes Red (All-red phase)
//            SetLaneState(lane1Waypoints, Waypoint.State.Red);
//            SetLaneState(lane2Waypoints, Waypoint.State.Red);
//            SetLaneState(lane3Waypoints, Waypoint.State.Red);
//            yield return new WaitForSeconds(allRedDuration);
//        }
//    }

//    // Helper function to set colors of traffic lights in a lane
//    void SetLaneState(List<Waypoint> waypoints, Waypoint.State state)
//    {
//        foreach (Waypoint waypoint in waypoints)
//        {
//            if (waypoint != null)
//            {
//                waypoint.SetState(state);
//            }
//        }
//    }
//}
