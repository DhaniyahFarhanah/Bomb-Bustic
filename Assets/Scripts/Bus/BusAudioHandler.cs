using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BusAudioHandler : MonoBehaviour
{
    private AudioSource b_AudioSource;
    public AudioSource bgm_AudioSource1;
    public AudioSource bgm_AudioSource2;
    private AudioSource extraAudioSource;
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
    //[SerializeField] public AudioClip MenuSoundtrack;
    [SerializeField] public AudioClip WinningSoundtrack;
    [SerializeField] public AudioClip PoliceAlert;
    [SerializeField] public AudioClip ShootPassenger;
    [SerializeField] public AudioClip TurretShot;
    [SerializeField] public AudioClip Shockwave;
    [SerializeField] public AudioClip Emp;
    [SerializeField] public AudioClip Boost;
    [SerializeField] public AudioClip BombRedZone;
    [SerializeField] public AudioClip BombDeadZone;
    [SerializeField] public AudioClip BombExplosion;
    [SerializeField] public AudioClip Splat;
    [SerializeField] public AudioClip engineHum;

    [SerializeField] public DrivingCameraController camControl;
    //private bool isFading = false;
    private bool ActiveAudioSource = true;
    [HideInInspector]public bool sfxIsLooping = false;

    // Start is called before the first frame update
    void Start()
    {
        b_AudioSource = GetComponent<AudioSource>();
        b_AudioSource.volume = volumeSFX;

        extraAudioSource = gameObject.AddComponent<AudioSource>();
        extraAudioSource.volume = volumeSFX;

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
    }


    void Update() 
    {
        if(ActiveAudioSource && DrivingSoundtrack.Length > 1) 
        {
            bgm_AudioSource2.volume = volumebgm;
            bgm_AudioSource2.clip = DrivingSoundtrack[1];
            bgm_AudioSource2.PlayDelayed(DrivingSoundtrack[0].length - 0.01f);
            bgm_AudioSource2.loop = true;
            ActiveAudioSource = false;
        }
    }
    public void Play(AudioClip clip)
    {
        if(b_AudioSource.isPlaying)
        {
            return;
        }
        b_AudioSource.loop = false;
        b_AudioSource.clip = clip;
        b_AudioSource.Play();
    }
    public void PlayPriority(AudioClip clip)
    {
        if(b_AudioSource.isPlaying)
        {
            b_AudioSource.Stop();
        }
        b_AudioSource.clip = clip;
        b_AudioSource.Play();
    }

    public void Play(AudioClip[] clips)
    {
        if(b_AudioSource.isPlaying)
        {
            return;
        }
        b_AudioSource.clip = clips[Random.Range(0, clips.Length)];
        b_AudioSource.Play();
    }

    

    public void PauseGameBGM() {
        bgm_AudioSource2.Pause();
    }

    public void ResumeGameBGM() {
        bgm_AudioSource2.Play();
    }

    public void PlaySFXLoop(AudioClip clip)
    {
        b_AudioSource.clip = clip;
        b_AudioSource.loop = true;
        b_AudioSource.Play();
        sfxIsLooping = true;
    }

    public void StopSFXLoop()
    {
        b_AudioSource.loop = false;
        b_AudioSource.Stop();
        sfxIsLooping = false;
    }

    public void PlayOneShotSFX(AudioClip clip)
    {
        extraAudioSource.volume = volumeSFX;
        extraAudioSource.clip = clip;
        extraAudioSource.Play();
    }
}
