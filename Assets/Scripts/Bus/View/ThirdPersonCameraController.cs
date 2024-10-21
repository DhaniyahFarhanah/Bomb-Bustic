using UnityEngine;

namespace ArcadeVehicleController
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject m_CameraHolder;
        [SerializeField] private float m_Distance = 10.0f;
        [SerializeField] private float m_Height = 5.0f;
        [SerializeField] private float m_HeightDamping = 2.0f;
        [SerializeField] private float m_MoveSpeed = 1.0f;
        [SerializeField] private float m_NormalFov = 60.0f;
        [SerializeField] private float m_FastFov = 90.0f;
        [SerializeField] private float m_FovDampingSpeeding = 0.25f;
        [SerializeField] private float m_FovDampingSlowing = 0.25f;
        [SerializeField] private float activateFovVelocity;
        [SerializeField] private float m_MouseSensitivity = 100.0f;
        [SerializeField] private float m_MaxPitchAngle = 80.0f;  // Maximum look-up angle
        [SerializeField] private float m_MinPitchAngle = -30.0f; // Maximum look-down angle

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

        private void HandleCameraPosition()
        {
            float wantedHeight = FollowTarget.position.y + m_Height;
            float currentHeight = m_Transform.position.y;

            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, m_HeightDamping * Time.deltaTime);

            // Apply both yaw (horizontal) and pitch (vertical) rotations to the camera
            Quaternion rotation = Quaternion.Euler(m_PitchRotation, m_YawRotation, 0);

            // Calculate the desired position based on the rotation and distance from the car
            Vector3 desiredPosition = FollowTarget.position - rotation * Vector3.forward * m_Distance;
            desiredPosition.y = currentHeight;

            // Smoothly move the camera to the desired position
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);

            // Rotate the camera manually instead of using LookAt to ensure pitch control
            m_Transform.rotation = rotation;
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
