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

        private Transform m_Transform;
        private Camera m_Camera;
        public Transform FollowTarget { get; set; }
        public float SpeedRatio { get; set; }

        private float m_YawRotation;
        private float m_PitchRotation;

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
            // Get mouse input and adjust the yaw and pitch rotation
            float mouseX = Input.GetAxis("Mouse X") * m_MouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * m_MouseSensitivity * Time.deltaTime;

            // Adjust yaw rotation (left-right rotation)
            m_YawRotation += mouseX;

            // Adjust pitch rotation (up-down rotation) and clamp it within allowed angles
            m_PitchRotation -= mouseY;
            m_PitchRotation = Mathf.Clamp(m_PitchRotation, m_MinPitchAngle, m_MaxPitchAngle);
        }

        private void HandleCameraZoom()
        {
            // Get the scroll wheel input to zoom in/out
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            // Adjust the distance based on scroll input
            m_Distance -= scrollInput * m_ScrollSensitivity;

            // Clamp the distance to the allowed min and max values
            m_Distance = Mathf.Clamp(m_Distance, m_MinDistance, m_MaxDistance);
        }

        private void HandleCameraPosition()
        {
            // Use the car's position as a base for the camera position
            Vector3 followPosition = FollowTarget.position;

            // Extract only the yaw (Y-axis) rotation from the car, ignoring roll (Z-axis) and pitch (X-axis)
            Quaternion followRotation = Quaternion.Euler(0, FollowTarget.eulerAngles.y, 0);

            // Apply the camera's mouse-controlled yaw and pitch rotation on top of the car's yaw rotation
            Quaternion combinedRotation = followRotation * Quaternion.Euler(m_PitchRotation, m_YawRotation, 0);

            // Calculate the desired position based on the car's position and the combined rotation
            Vector3 desiredPosition = followPosition - combinedRotation * Vector3.forward * m_Distance;
            desiredPosition.y += m_Height;

            // Smoothly move the camera to the desired position
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);

            // Apply the combined rotation to the camera to follow the car's yaw while allowing mouse input for looking around
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
