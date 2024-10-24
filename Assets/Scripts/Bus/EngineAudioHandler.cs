using System.Collections;
using System.Collections.Generic;
using ArcadeVehicleController;
using UnityEngine;

public class EngineAudioHandler : MonoBehaviour
{
    private AudioSource _AudioSource;
    public AudioClip engineLoop;
    public float maxSpeed = 80.0f;
    public float minPitch = 0.1f;
    public float maxPitch = 1.0f;
    public float engineVolume = 0.7f;   
    private GameObject _bus;

    // Start is called before the first frame update
    void Start()
    {
        _bus = GameObject.FindGameObjectWithTag("Player");
        _AudioSource = GetComponent<AudioSource>();
        _AudioSource.volume = engineVolume;
        _AudioSource.clip = engineLoop;
        _AudioSource.loop = true;
        _AudioSource.Play();
    }
    
    void Update()
    {
        BombMeter _bombMeter = _bus.GetComponent<BombMeter>();
        if(_bombMeter.GetCurrentSpeed() > 0.0f)
        {
            AdjustPitch();
        }
    }

    void AdjustPitch()
    {
        //float maxSpeed = _bus.GetComponent<Vehicle>().MaxSpeed;        
        float pitch = Mathf.Lerp(minPitch, maxPitch, _bus.GetComponent<BombMeter>().GetCurrentSpeed()/maxSpeed);

        _AudioSource.pitch = pitch;
    }
    
}
