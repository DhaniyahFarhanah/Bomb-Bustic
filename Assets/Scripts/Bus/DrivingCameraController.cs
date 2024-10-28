using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace ArcadeVehicleController
{
    public class DrivingCameraController: MonoBehaviour
    {
        [SerializeField] private GameObject m_CameraHolder;
        [SerializeField] private float m_Distance = 10.0f;
        [SerializeField] private float m_Height = 5.0f;
        [SerializeField] private float m_HeightDamping = 2.0f;
        [SerializeField] private float m_RotationDamping = 3.0f;
        [SerializeField] private float m_MoveSpeed = 1.0f;
        [SerializeField] private float m_NormalFov = 60.0f;
        [SerializeField] private float m_FastFov = 90.0f;
        public float fovAdd = 0f;
        [SerializeField] private float m_FovDampingSpeeding = 0.25f;
        [SerializeField] private float m_FovDampingSlowing = 0.25f;
        [SerializeField] private float m_Offset = 0.0f;
        [SerializeField] private float activateFovVelocity;

        //test run. remove if idea doesn't pan out. will change to another script if it does
        [Header("Missile Test")]
        RaycastHit hit;
        private Vector3 targetPos;
        [SerializeField] LayerMask drivable;
        [SerializeField] GameObject Crosshair;
        [SerializeField] GameObject TurretHead;
        [SerializeField] GameObject MissilePrefab;
        [SerializeField] float ShootCooldown;
        float currentTimer;

        [Header("Norm Values")]
        [SerializeField] private float m_DistanceNorm = 10.0f;
        [SerializeField] private float m_HeightNorm = 5.0f;
        [SerializeField] private float m_OffsetNorm = 0.0f;


        [Header("Turret Values")]
        [SerializeField] private float m_DistanceTurret = 10.0f;
        [SerializeField] private float m_HeightTurret = 5.0f;
        [SerializeField] private float m_OffsetTurret = 0.0f;

        private Transform m_Transform;
        private Camera m_Camera;
        public Transform FollowTarget { get; set; }
        public float SpeedRatio { get; set; }

        public enum CameraModes
        {
            Normal,
            Turret,
            PassengerEject
        }
        public CameraModes cameraMode;

        private void Awake()
        {
            m_Transform = transform;
            m_Camera = m_CameraHolder.GetComponent<Camera>();
        }

        public void LateUpdate()
        {
            if (FollowTarget == null)
            {
                return;
            }

            CameraMode(); //delete if it doesnt work

            HandleCameraPosition(); 
            HandleFOV();

        }

        void HandleCameraPosition()
        {
            float wantedRotationAngle = FollowTarget.eulerAngles.y;
            float wantedHeight = FollowTarget.position.y + m_Height;
            float currentRotationAngle = m_Transform.eulerAngles.y;
            float currentHeight = m_Transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, m_RotationDamping * Time.unscaledDeltaTime);

            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, m_HeightDamping * Time.unscaledDeltaTime);

            Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

            Vector3 desiredPosition = FollowTarget.position;
            desiredPosition -= currentRotation * Vector3.forward * m_Distance;
            desiredPosition.y = currentHeight;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.unscaledDeltaTime * m_MoveSpeed);

            Vector3 targetWithOffset = new Vector3(FollowTarget.position.x, FollowTarget.position.y + m_Offset, FollowTarget.position.z);

            m_Transform.LookAt(targetWithOffset);

            //const float FAST_SPEED_RATIO = 0.9f;
            //float targetFov = SpeedRatio > FAST_SPEED_RATIO ? m_FastFov : m_NormalFov;
            //m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, targetFov, Time.deltaTime * m_FovDampingSlowing);

            if (FollowTarget == null)
                return;
        }

        void HandleFOV()
        {
            if (FollowTarget.GetComponent<Rigidbody>().velocity.magnitude > activateFovVelocity)
            {
                if (m_Camera.fieldOfView < m_FastFov)
                {
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_FastFov + fovAdd, Time.unscaledDeltaTime * m_FovDampingSpeeding);
                }
                else if (m_Camera.fieldOfView > m_FastFov + fovAdd)
                {
                    m_Camera.fieldOfView = m_FastFov + fovAdd;
                }
            }

            else if (FollowTarget.GetComponent<Rigidbody>().velocity.magnitude < activateFovVelocity)
            {
                if (m_Camera.fieldOfView > m_NormalFov)
                {
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_NormalFov, Time.unscaledDeltaTime * m_FovDampingSlowing);
                }
                else
                {
                    m_Camera.fieldOfView = m_FastFov + fovAdd;
                }
            }
        }

        //test run with turret idea. Delete if it doesn't pan out
        void CameraMode()
        {
            switch (cameraMode)
            {
                case CameraModes.Normal:
                    m_Distance = m_DistanceNorm;
                    m_Height = m_HeightNorm;
                    m_Offset = m_OffsetNorm;
                    Crosshair.SetActive(false);
                    break;

                case CameraModes.Turret:
                    m_Distance = m_DistanceTurret;
                    m_Height = m_HeightTurret;
                    m_Offset = m_OffsetTurret;
                    Crosshair.SetActive(true);
                    CursorAim();
                    break;

                case CameraModes.PassengerEject:

                    break;
            }
        }

        void CursorAim()
        {
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            targetPos = m_Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_Camera.transform.position.z));
            Crosshair.transform.position = Input.mousePosition;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, drivable))
            {
                Vector3 lookDir = hit.point - TurretHead.transform.position;
                lookDir.y = 0;
                TurretHead.transform.forward = lookDir;
            }

            currentTimer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0) && currentTimer >= ShootCooldown)
            {
                currentTimer = 0;
                //this is if you wanna get the bullets to home in to the object
                if (Physics.Raycast(ray, out hit, 300.0f))
                {
                    GameObject Missile = Instantiate(MissilePrefab, TurretHead.transform.position, TurretHead.transform.rotation);
                    //Missile.GetComponent<Missile>().target = hit.point;
                }
            }
            //cooldown
        }
    }  
}