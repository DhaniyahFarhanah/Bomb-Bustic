﻿using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;

namespace ArcadeVehicleController
{
    //Jeep visual is the wheel turning. It's purely visual

    public class JeepVisual : MonoBehaviour
    {
        public bool debugLines = false;
        public bool isDrift = false;

        [Header("Wheel Visuals")]
        [SerializeField] private Transform m_WheelFrontLeft;
        [SerializeField] private Transform m_WheelFrontRight;
        [SerializeField] private Transform m_WheelBackLeft;
        [SerializeField] private Transform m_WheelBackLeft2;
        [SerializeField] private Transform m_WheelBackRight;
        [SerializeField] private Transform m_WheelBackRight2;
        [SerializeField] private float m_WheelsSpinSpeed;
        [SerializeField] private float m_WheelYWhenSpringMin;
        [SerializeField] private float m_WheelYWhenSpringMax;

        [Header("Particle Visuals")]
        [SerializeField] private float m_CheckHeight;
        [SerializeField] private LayerMask m_Ground;
        [SerializeField] private float highSpeed;
        [SerializeField] private float skidDelay;

        [Header("Right Wheel")]
        [SerializeField] private Transform m_RightWheelGround;
        [SerializeField] private TrailRenderer m_TrialRight;
        [SerializeField] private ParticleSystem m_DustRight;
        [SerializeField] private ParticleSystem m_DriftDustRight;
        [SerializeField] private TrailRenderer m_DriftTrialRight;

        [Header("Left Wheel")]
        [SerializeField] private Transform m_LeftWheelGround;
        [SerializeField] private TrailRenderer m_TrialLeft;
        [SerializeField] private ParticleSystem m_DustLeft;
        [SerializeField] private ParticleSystem m_DriftDustLeft;
        [SerializeField] private TrailRenderer m_DriftTrailLeft;

        private float currentTime;
        private Quaternion m_WheelFrontLeftRoll;
        private Quaternion m_WheelFrontRightRoll;

        [Header("Visuals")]
        public Material brakeLights;


        public bool IsLeftGrounded { get; set; }
        public bool IsRightGrounded { get; set; }
        public bool IsMovingForward { get; set; }

        public float ForwardSpeed { get; set; }

        public float SteerInput { get; set; }
        public float BrakeInput { get; set; }

        public float SteerAngle { get; set; }

        public float SpringsRestLength { get; set; }

        public Dictionary<Wheel, float> SpringsCurrentLength { get; set; } = new()
        {
            { Wheel.FrontLeft, 0.0f },
            { Wheel.FrontRight, 0.0f },
            { Wheel.BackLeft, 0.0f },
            { Wheel.BackRight, 0.0f }
        };

        private void Start()
        {
            m_WheelFrontLeftRoll = m_WheelFrontLeft.localRotation;
            m_WheelFrontRightRoll = m_WheelFrontRight.localRotation;
        }

        private void Update()
        {
            IsLeftGrounded = Physics.Raycast(m_LeftWheelGround.position, -m_LeftWheelGround.up, m_CheckHeight, m_Ground);
            IsRightGrounded = Physics.Raycast(m_RightWheelGround.position, -m_RightWheelGround.up, m_CheckHeight, m_Ground);

            if (debugLines)
            {
                Debug.DrawLine(m_LeftWheelGround.position, m_LeftWheelGround.position + -m_LeftWheelGround.up * m_CheckHeight, Color.blue);
                Debug.DrawLine(m_RightWheelGround.position, m_RightWheelGround.position + -m_RightWheelGround.up * m_CheckHeight, Color.blue);
            }

            if (SpringsCurrentLength[Wheel.FrontLeft] < SpringsRestLength)
            {
                m_WheelFrontLeftRoll *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            if (SpringsCurrentLength[Wheel.FrontRight] < SpringsRestLength)
            {
                m_WheelFrontRightRoll *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            if (SpringsCurrentLength[Wheel.BackLeft] < SpringsRestLength)
            {
                m_WheelBackLeft.localRotation *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
                m_WheelBackLeft2.localRotation *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            if (SpringsCurrentLength[Wheel.BackRight] < SpringsRestLength)
            {
                m_WheelBackRight.localRotation *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
                m_WheelBackRight2.localRotation *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            m_WheelFrontLeft.localRotation = Quaternion.AngleAxis(SteerInput * SteerAngle, Vector3.up) * m_WheelFrontLeftRoll;
            m_WheelFrontRight.localRotation = Quaternion.AngleAxis(SteerInput * SteerAngle, Vector3.up) * m_WheelFrontRightRoll;

            float springFrontLeftRatio = SpringsCurrentLength[Wheel.FrontLeft] / SpringsRestLength;
            float springFrontRightRatio = SpringsCurrentLength[Wheel.FrontRight] / SpringsRestLength;
            float springBackLeftRatio = SpringsCurrentLength[Wheel.BackLeft] / SpringsRestLength;
            float springBackRightRatio = SpringsCurrentLength[Wheel.BackRight] / SpringsRestLength;

            m_WheelFrontLeft.localPosition = new Vector3(m_WheelFrontLeft.localPosition.x,
                m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springFrontLeftRatio,
                m_WheelFrontLeft.localPosition.z);

            m_WheelFrontRight.localPosition = new Vector3(m_WheelFrontRight.localPosition.x,
                m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springFrontRightRatio,
                m_WheelFrontRight.localPosition.z);

            m_WheelBackRight.localPosition = new Vector3(m_WheelBackRight.localPosition.x,
                m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springBackRightRatio,
                m_WheelBackRight.localPosition.z);

            m_WheelBackRight2.localPosition = new Vector3(m_WheelBackRight2.localPosition.x,
                m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springBackRightRatio,
                m_WheelBackRight2.localPosition.z);

            m_WheelBackLeft.localPosition = new Vector3(m_WheelBackLeft.localPosition.x,
                m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springBackLeftRatio,
                m_WheelBackLeft.localPosition.z);

            m_WheelBackLeft2.localPosition = new Vector3(m_WheelBackLeft2.localPosition.x,
                m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springBackLeftRatio,
                m_WheelBackLeft2.localPosition.z);

            //Particle Visuals
            HandleParticles();

        }

        void HandleParticles()
        {
            //high speed smoke
            if(ForwardSpeed > highSpeed)
            {
                SpeedSmoke(true);
            }

            //Turning at mid speed
            else if ((ForwardSpeed > 20.0f && (SteerInput > 0.05f || SteerInput < -0.05f)))
            {
                if(currentTime <= skidDelay)
                {
                    currentTime -= Time.deltaTime;
                }

                if(currentTime < 0.0f)
                {
                    SpeedSmoke(true);
                    TrailEffect(true);
                    
                }
            }

            //Reversing
            else if(ForwardSpeed < 0.0f || BrakeInput < 0.0f)
            {
                TrailEffect(true);
            }
            else if(ForwardSpeed > -5.0f && ForwardSpeed < 0.0f)
            {
                SpeedSmoke(true);
            }

            //drifting
            else if (isDrift)
            {
                TrailEffect(true);
                DriftSmoke(true);
            }

            //on sand

            //revert
            else
            {
                SpeedSmoke(false); 
                TrailEffect(false);
                DriftSmoke(false);
                currentTime = skidDelay;
            }
        }

        void TrailEffect(bool activate)
        {
            if (activate)
            {
                m_TrialLeft.emitting = IsLeftGrounded;
                m_TrialRight.emitting = IsRightGrounded;
            }

            else if (!activate)
            {
                m_TrialLeft.emitting = false;
                m_TrialRight.emitting = false;
            }
        }

        void SpeedSmoke(bool activate)
        {
            if (activate)
            {
                if (IsLeftGrounded)
                {
                    m_DustLeft.Play(true);
                }
                else
                {
                    m_DustLeft.Stop(true);
                }

                if (IsRightGrounded)
                {
                    m_DustRight.Play(true);
                }
                else
                {
                    m_DustRight.Stop(true);
                }
            }
            else
            {
                m_DustLeft.Stop();
                m_DustRight.Stop();
            }
        }

        void DriftSmoke(bool activate)
        {
            if(SteerInput > 0f && activate)
            {
                m_DriftTrialRight.emitting = IsRightGrounded;
            }
            else if(SteerInput < 0f && activate)
            {
                m_DriftTrailLeft.emitting = IsLeftGrounded;
            }

            else if (!activate)
            {
                m_DriftTrailLeft.emitting = false;
                m_DriftTrialRight.emitting = false;
            }

            else
            {
                m_DriftTrailLeft.emitting = IsLeftGrounded;
                m_DriftTrialRight.emitting = IsRightGrounded;
            }

            if (activate)
            {
                if (IsLeftGrounded)
                {
                    m_DriftDustLeft.Play(true);
                }
                else
                {
                    m_DriftDustLeft.Stop(true);
                }

                if (IsRightGrounded)
                {
                    m_DriftDustRight.Play(true);
                }
                else
                {
                    m_DriftDustRight.Stop(true);
                }
            }
            else
            {
                m_DriftDustLeft.Stop();
                m_DriftDustRight.Stop();
            }
        }
    }
}