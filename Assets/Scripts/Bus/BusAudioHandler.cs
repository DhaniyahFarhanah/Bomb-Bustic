using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BusAudioHandler : MonoBehaviour
{
    [SerializeField] public AudioSource b_AudioSourceSFX1;
    [SerializeField] private GameObject beepEffect;
    private AudioSource b_BeepSource;
    public AudioSource bgm_AudioSource1;
    public AudioSource bgm_AudioSource2;
    public AudioSource extraAudioSource;
    public AudioSource collisionSounds;
    public AudioSource skidSound;
    [SerializeField] float volumebgm = 0.5f;
    [SerializeField] float volumeSFX = 1.0f;
    //[SerializeField] float fadeTime = 2.0f;
    [SerializeField] public AudioClip[] sCrash;
    [SerializeField] public AudioClip mCrash;
    [SerializeField] public AudioClip lCrash;
    [SerializeField] public AudioClip lose;
    [SerializeField] public AudioClip pickUp;
    [SerializeField] public AudioClip passengerWee;
    [SerializeField] public AudioClip passengerScream; 
    [SerializeField] public AudioClip passengerDisgust;
    [SerializeField] public AudioClip flush;
    [SerializeField] public AudioClip win;   
    [SerializeField] public AudioClip[] DrivingSoundtrack;
    [SerializeField] public AudioClip fastTick;
    [SerializeField] public AudioClip midTick;
    [SerializeField] public AudioClip slowTick;
    //[SerializeField] public AudioClip MenuSoundtrack;
    [SerializeField] public AudioClip WinningSoundtrack;
    [SerializeField] public AudioClip PoliceAlert;
    [SerializeField] public AudioClip ShootPassenger;
    [SerializeField] public AudioClip TurretShot;
    [SerializeField] public AudioClip Shockwave;
    [SerializeField] public AudioClip IceBreak;
    [SerializeField] public AudioClip Emp;
    [SerializeField] public AudioClip Boost;
    [SerializeField] public AudioClip BoostDown;
    [SerializeField] public AudioClip BombRedZone;
    [SerializeField] public AudioClip BombDeadZone;
    [SerializeField] public AudioClip BombExplosion;
    [SerializeField] public AudioClip Splat;
    [SerializeField] public AudioClip engineHum;
    [SerializeField] public AudioClip Beep;
    [SerializeField] public AudioClip TireScreech;
    [SerializeField] public AudioClip MetalScreech;
    [SerializeField] public AudioClip EngineHiss;

    [SerializeField] public DrivingCameraController camControl;
    //private bool isFading = false;
    private bool ActiveAudioSource = true;
    [HideInInspector]public bool sfxIsLooping = false;

    // Start is called before the first frame update
    void Start()
    {
        b_AudioSourceSFX1 = GetComponent<AudioSource>();
        b_AudioSourceSFX1.volume = volumeSFX;

        extraAudioSource = gameObject.AddComponent<AudioSource>();
        extraAudioSource.volume = volumeSFX;

        skidSound = gameObject.AddComponent<AudioSource>();
        skidSound.volume = volumeSFX;
        skidSound.loop = false;
        skidSound.clip = TireScreech;
        skidSound.Stop();

        collisionSounds = gameObject.AddComponent<AudioSource>();
        collisionSounds.volume = volumeSFX;
        collisionSounds.loop = false;
        collisionSounds.clip = TireScreech;
        collisionSounds.Stop();

        b_BeepSource = gameObject.AddComponent<AudioSource>();
        b_BeepSource.volume = volumeSFX;
        b_BeepSource.loop = true;
        b_BeepSource.clip = Beep;
        b_BeepSource.Stop();


        GameObject PlayerCamera = GameObject.Find("Player Camera");
        bgm_AudioSource1 = PlayerCamera.GetComponent<AudioSource>();
        bgm_AudioSource2 = PlayerCamera.AddComponent<AudioSource>();

        // Check if DrivingSoundtrack has enough elements
        if (DrivingSoundtrack.Length > 0)
        {
            bgm_AudioSource1.clip = DrivingSoundtrack[0];
            bgm_AudioSource1.volume = volumebgm;
            bgm_AudioSource1.Play();
        }
        else
        {
            Debug.LogWarning("DrivingSoundtrack array is empty. Please assign audio clips.");
        }

        if (DrivingSoundtrack.Length > 1)
        {
            bgm_AudioSource2.volume = volumebgm;
            bgm_AudioSource2.clip = DrivingSoundtrack[1];
        }
        else
        {
            Debug.LogWarning("DrivingSoundtrack array has fewer than 2 elements. Please assign additional audio clips.");
        }
    }


    void Update() {
        if(ActiveAudioSource && !bgm_AudioSource1.isPlaying) {
            ActiveAudioSource  = false;
            bgm_AudioSource2.loop = true;
            bgm_AudioSource2.Play();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            b_BeepSource.pitch = Random.Range(0.99f, 1.01f);
            Instantiate(beepEffect, gameObject.transform);
            b_BeepSource.Play();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            b_BeepSource.Stop();
        }
    }
    public void Play(AudioClip clip)
    {
        if(b_AudioSourceSFX1.isPlaying)
        {
            return;
            
        }

        b_AudioSourceSFX1.loop = false;
        b_AudioSourceSFX1.clip = clip;
        b_AudioSourceSFX1.Play();
    }
    public void PlayPriority(AudioClip clip)
    {
        if(b_AudioSourceSFX1.isPlaying)
        {
            b_AudioSourceSFX1.Stop();
        }
        b_AudioSourceSFX1.clip = clip;
        b_AudioSourceSFX1.Play();
    }

    public void Play(AudioClip[] clips)
    {
        if(b_AudioSourceSFX1.isPlaying)
        {
            return;
        }
        b_AudioSourceSFX1.clip = clips[Random.Range(0, clips.Length)];
        b_AudioSourceSFX1.Play();
    }

    

    public void PauseGameBGM() {
        bgm_AudioSource2.Pause();
    }

    public void ResumeGameBGM() {
        bgm_AudioSource2.Play();
    }

    public void PlaySFXLoop(AudioClip clip)
    {
        b_AudioSourceSFX1.clip = clip;
        b_AudioSourceSFX1.loop = true;
        b_AudioSourceSFX1.Play();
        sfxIsLooping = true;
    }

    public void StopSFXLoop()
    {
        b_AudioSourceSFX1.loop = false;
        b_AudioSourceSFX1.Stop();
        sfxIsLooping = false;
    }

    public void PlayOneShotSFX(AudioClip clip)
    {
        extraAudioSource.volume = volumeSFX;
        extraAudioSource.clip = clip;
        extraAudioSource.Play();
    }

    public void CollisionSounds(AudioClip clip)
    {
        collisionSounds.volume = volumeSFX;
        collisionSounds.clip = clip;
        collisionSounds.Play();
    }
}
