using TMPro;
using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeVehicleController
{
    public enum CameraModes
    {
        Normal,
        Turret,
        PassengerEject
    }

    public class DrivingCameraController: MonoBehaviour
    {
        private CameraModes cameraMode;

        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameObject m_CameraHolder;
        [SerializeField] private BusAudioHandler m_BusAudioHandler;
        [SerializeField] private float m_Distance = 10.0f;
        [SerializeField] private float m_Height = 5.0f;
        [SerializeField] private float m_HeightDamping = 2.0f;
        [SerializeField] private float m_RotationDamping = 3.0f;
        [SerializeField] private float m_MoveSpeed = 1.0f;
        [SerializeField] private float m_NormalFov = 60.0f;
        [SerializeField] private float m_Offset = 0.0f;

        [Header("FOV")]
        [SerializeField] private float m_FovDampingSpeeding = 0.25f;
        [SerializeField] private float m_FovDampingSlowing = 0.25f;
        [SerializeField] private float m_SpeedToActivateFOV = 50.0f;    //min Speed to activate FOV change
        [SerializeField] private float m_MaxSpeedForFOV = 90.0f;        //max Speed for max FOV
        [SerializeField] private float m_FastFOVIncrease = 20.0f;       //amt of fov added 
        [SerializeField] private float m_ratioFOV;
        public float nitroFOVIncrease = 0f;

        //test run. remove if idea doesn't pan out. will change to another script if it does

        [Header("Norm Values")]
        [SerializeField] private float m_DistanceNorm = 10.0f;
        [SerializeField] private float m_HeightNorm = 5.0f;
        [SerializeField] private float m_OffsetNorm = 0.0f;


        [Header("Turret Values")]
        [SerializeField] private float m_DistanceTurret = 10.0f;
        [SerializeField] private float m_HeightTurret = 5.0f;
        [SerializeField] private float m_OffsetTurret = 0.0f;
        [SerializeField] float ShootCooldown;

        [Header("Missile Test")]
        [SerializeField] LayerMask drivable;
        [SerializeField] GameObject Crosshair;
        [SerializeField] GameObject TurretHead;
        [SerializeField] GameObject MissilePrefab;
        RaycastHit hit;
        private Vector3 targetPos;
        float currentTimer;

        private Transform m_Transform;
        private Camera m_Camera;
        public Transform FollowTarget { get; set; }
        public float SpeedRatio { get; set; }

        [Header("Passenger Ejection")]
        [SerializeField] private float m_TransitionDuration = 1.0f;
        [SerializeField] private float m_MouseSensitivity = 100.0f;
        [SerializeField] private float m_MaxPitchAngle = 80.0f;     // Maximum look-up angle
        [SerializeField] private float m_MinPitchAngle = -30.0f;    // Maximum look-down angle
        private float m_YawRotation;
        private float m_PitchRotation;
        private float cameraDiff;
        private float cameraDiffStart;

        [SerializeField] [Range(0.0f, 1.0f)] private float m_TimeDilation;              //time scale for slowmo
        [SerializeField] private float m_slowMotionTransitionSpeed;                     //Time ratio


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

            CameraMode();
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
            m_Transform.position = Vector3.Lerp(m_Transform.position, desiredPosition, Time.unscaledDeltaTime * m_MoveSpeed);

            Vector3 targetWithOffset = new Vector3(FollowTarget.position.x, FollowTarget.position.y + m_Offset, FollowTarget.position.z);

            m_Transform.LookAt(targetWithOffset);

            //const float FAST_SPEED_RATIO = 0.9f;
            //float targetFov = SpeedRatio > FAST_SPEED_RATIO ? m_FastFov : m_NormalFov;
            //m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, targetFov, Time.deltaTime * m_FovDampingSlowing);

            if (FollowTarget == null)
                return;
        }

        /*void HandleFOV()
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
        }*/

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
                    HandleCameraPosition();
                    HandleFov();

                    m_YawRotation = 0f;
                    m_PitchRotation = 0f;

                    //time dilation
                    ResetTimeDilation();
                    break;

                case CameraModes.Turret:
                    m_Distance = m_DistanceTurret;
                    m_Height = m_HeightTurret;
                    m_Offset = m_OffsetTurret;
                    Crosshair.SetActive(true);
                    CursorAim();
                    HandleCameraPosition();
                    HandleFov();

                    m_YawRotation = 0f;
                    m_PitchRotation = 0f;

                    //time dilation
                    ResetTimeDilation();
                    break;

                case CameraModes.PassengerEject:
                    m_Distance = m_DistanceNorm;
                    m_Height = m_HeightNorm;
                    m_Offset = m_OffsetNorm;
                    Crosshair.SetActive(false);
                    HandleMouseRotation2();
                    HandleCameraPosition2();
                    HandleFov();

                    //time dilation
                    TimeDilation();
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

        private void HandleMouseRotation2()
        {
            float mouseX = Input.GetAxis("Mouse X") * m_MouseSensitivity * Time.unscaledDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * m_MouseSensitivity * Time.unscaledDeltaTime;

            if (mouseX != 0 || mouseY != 0)
            {
                m_YawRotation += mouseX;
                m_PitchRotation -= mouseY;
                m_PitchRotation = Mathf.Clamp(m_PitchRotation, m_MinPitchAngle, m_MaxPitchAngle);
            }
        }

        private void HandleCameraPosition2()
        {
            Vector3 followPosition = FollowTarget.position;
            Quaternion followRotation = Quaternion.Euler(0, FollowTarget.eulerAngles.y, 0);

            Quaternion combinedRotation;

            // Normal mouse-controlled rotation
            combinedRotation = followRotation * Quaternion.Euler(m_PitchRotation, m_YawRotation, 0);
            
            Vector3 desiredPosition = followPosition - combinedRotation * Vector3.forward * m_DistanceNorm;
            desiredPosition.y += m_Height;

            m_Transform.position = Vector3.Lerp(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);
            m_Transform.rotation = combinedRotation;

            Quaternion temp = Quaternion.Euler(transform.eulerAngles.x + cameraDiff, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.eulerAngles = temp.eulerAngles;
        }

        private void HandleFov()
        {
            float velocityMagnitude = FollowTarget.GetComponent<Rigidbody>().velocity.magnitude;

            if (velocityMagnitude > m_SpeedToActivateFOV)
            {
                float ratio = ((velocityMagnitude - m_SpeedToActivateFOV) / (m_MaxSpeedForFOV - m_SpeedToActivateFOV) * 1f);


                if (ratio < 1f)
                {
                    m_ratioFOV = (ratio * m_FastFOVIncrease) + m_NormalFov;
                }

                else if (ratio > 1f)
                {

                    m_ratioFOV = m_FastFOVIncrease + m_NormalFov;
                }

                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_ratioFOV + nitroFOVIncrease, Time.unscaledDeltaTime * m_FovDampingSpeeding);
            }

            else if (velocityMagnitude < m_SpeedToActivateFOV)
            {
                if (m_Camera.fieldOfView > m_NormalFov)
                {
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_NormalFov + nitroFOVIncrease, Time.unscaledDeltaTime * m_FovDampingSlowing);
                }
                else
                {
                    m_Camera.fieldOfView = m_NormalFov;
                }
            }
        }
        public void SetCameraMode(CameraModes mode)
        {

            if (mode == CameraModes.PassengerEject)
            {
                cameraDiff = transform.eulerAngles.x;
                cameraDiffStart = cameraDiff;
                cameraMode = mode;
                StartCoroutine(TransistionCamera(true));
            }
            else if (cameraMode == CameraModes.PassengerEject && mode == CameraModes.Normal)
            {
                StartCoroutine(TransistionCamera(false));
            }
            else
            {
                cameraMode = mode;
            }
        }

        private IEnumerator TransistionCamera(bool toggle)
        {
            float elapsedTime = m_TransitionDuration; // Total time for the transition
            float duration = elapsedTime; // Store the original duration

            while (elapsedTime > 0f)
            {
                elapsedTime -= Time.unscaledDeltaTime;

                // Interpolate funnyVar from startValue to 0 over the duration
                if (toggle)
                {
                    cameraDiff = Mathf.Lerp(0, cameraDiffStart, elapsedTime / duration);
                }
                else
                {
                    cameraDiff = Mathf.Lerp(cameraDiffStart, 0, elapsedTime / duration);
                }

                yield return null;
            }

            if (toggle)
            {
                cameraDiff = 0f;
            }
            else
            {
                cameraDiff = cameraDiffStart;
                cameraMode = CameraModes.Normal;
            }
        }

        public void TimeDilation()
        {
            if (!uiManager.m_IsPaused)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, m_TimeDilation, Time.deltaTime * m_slowMotionTransitionSpeed);
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Maintain consistent fixed time step during slow motion

                //maybe slow down music
                m_BusAudioHandler.bgm_AudioSource1.pitch = Mathf.Lerp(m_BusAudioHandler.bgm_AudioSource1.pitch, 0.7f, Time.unscaledDeltaTime * m_slowMotionTransitionSpeed);
                m_BusAudioHandler.bgm_AudioSource2.pitch = Mathf.Lerp(m_BusAudioHandler.bgm_AudioSource1.pitch, 0.7f, Time.unscaledDeltaTime * m_slowMotionTransitionSpeed);
            }
        }

        public void ResetTimeDilation()
        {
            if (!uiManager.m_IsPaused)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, Time.deltaTime * m_slowMotionTransitionSpeed);
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Maintain consistent fixed time step during slow motion

                m_BusAudioHandler.bgm_AudioSource1.pitch = Mathf.Lerp(m_BusAudioHandler.bgm_AudioSource1.pitch, 1f, Time.unscaledDeltaTime * m_slowMotionTransitionSpeed);
                m_BusAudioHandler.bgm_AudioSource2.pitch = Mathf.Lerp(m_BusAudioHandler.bgm_AudioSource1.pitch, 1f, Time.unscaledDeltaTime * m_slowMotionTransitionSpeed);
            }
        }
    }
}  