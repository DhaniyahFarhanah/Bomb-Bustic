using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeVehicleController
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject m_CameraHolder;
        [SerializeField] private float m_Distance = 10.0f;         // Initial camera distance
        [SerializeField] private float m_Height = 5.0f;
        [SerializeField] private float m_MoveSpeed = 1.0f;
        [SerializeField] private float m_NormalFov = 60.0f;
        [SerializeField] private float m_FastFov = 90.0f;
        [SerializeField] private float m_FovDampingSpeeding = 0.25f;
        [SerializeField] private float m_FovDampingSlowing = 0.25f;
        [SerializeField] private float activateFovVelocity;
        [SerializeField] private float m_MouseSensitivity = 100.0f;
        [SerializeField] private float m_MaxPitchAngle = 80.0f;    // Maximum look-up angle
        [SerializeField] private float m_MinPitchAngle = -30.0f;   // Maximum look-down angle
        [SerializeField] private float m_MaxDistance = 20.0f;      // Max camera zoom-out distance
        [SerializeField] private float m_MinDistance = 3.0f;       // Min camera zoom-in distance
        [SerializeField] private float m_ScrollSensitivity = 5.0f; // Sensitivity of the scroll wheel

        [SerializeField] private float idleTimeThreshold = 3.0f;   // Time in seconds before camera resets
        [SerializeField] private float returnSpeed = 2.0f;         // Speed at which camera returns to default

        private Transform m_Transform;
        private Camera m_Camera;
        public Transform FollowTarget { get; set; }
        public float SpeedRatio { get; set; }

        private float m_YawRotation;
        private float m_PitchRotation;

        private float idleTimer = 0.0f;  // Timer to track mouse inactivity
        private bool isReturningToDefault = false;

        private void Awake()
        {
            m_Transform = transform;
            m_Camera = m_CameraHolder.GetComponent<Camera>();

            // Lock the cursor for mouse camera control
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void LateUpdate()
        {
            if (FollowTarget == null)
            {
                return;
            }

            HandleMouseRotation();
            HandleCameraZoom();
            HandleCameraPosition();
            HandleCameraFOV();
        }

        private void HandleMouseRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * m_MouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * m_MouseSensitivity * Time.deltaTime;

            if (mouseX != 0 || mouseY != 0)
            {
                // Mouse moved, reset idle timer
                idleTimer = 0.0f;
                isReturningToDefault = false;
                m_YawRotation += mouseX;
                m_PitchRotation -= mouseY;
                m_PitchRotation = Mathf.Clamp(m_PitchRotation, m_MinPitchAngle, m_MaxPitchAngle);
            }
            else
            {
                // Mouse not moved, increment idle timer
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleTimeThreshold)
                {
                    // Start returning camera to default position
                    isReturningToDefault = true;
                }
            }
        }

        private void HandleCameraZoom()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            m_Distance -= scrollInput * m_ScrollSensitivity;
            m_Distance = Mathf.Clamp(m_Distance, m_MinDistance, m_MaxDistance);
        }

        private void HandleCameraPosition()
        {
            Vector3 followPosition = FollowTarget.position;
            Quaternion followRotation = Quaternion.Euler(0, FollowTarget.eulerAngles.y, 0);

            Quaternion combinedRotation;
            if (isReturningToDefault)
            {
                // Smoothly return to default behind-the-car camera position
                combinedRotation = Quaternion.Lerp(m_Transform.rotation, followRotation, returnSpeed * Time.deltaTime);
                m_PitchRotation = Mathf.Lerp(m_PitchRotation, 10.0f, returnSpeed * Time.deltaTime); // Default pitch (slightly above car)
                m_YawRotation = Mathf.Lerp(m_YawRotation, 0.0f, returnSpeed * Time.deltaTime);       // Default yaw (directly behind car)
            }
            else
            {
                // Normal mouse-controlled rotation
                combinedRotation = followRotation * Quaternion.Euler(m_PitchRotation, m_YawRotation, 0);
            }

            Vector3 desiredPosition = followPosition - combinedRotation * Vector3.forward * m_Distance;
            desiredPosition.y += m_Height;

            m_Transform.position = Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);
            m_Transform.rotation = combinedRotation;
        }

        private void HandleCameraFOV()
        {
            float velocityMagnitude = FollowTarget.GetComponent<Rigidbody>().velocity.magnitude;

            if (velocityMagnitude > activateFovVelocity)
            {
                if (m_Camera.fieldOfView < m_FastFov)
                {
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_FastFov, Time.deltaTime * m_FovDampingSpeeding);
                }
                else if (m_Camera.fieldOfView > m_FastFov)
                {
                    m_Camera.fieldOfView = m_FastFov;
                }
            }
            else if (velocityMagnitude < activateFovVelocity)
            {
                if (m_Camera.fieldOfView > m_NormalFov)
                {
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_NormalFov, Time.deltaTime * m_FovDampingSlowing);
                }
                else
                {
                    m_Camera.fieldOfView = m_NormalFov;
                }
            }
        }
    }
}
