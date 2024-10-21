using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // For sorting with LINQ
using TMPro;

public class VehicleShooting : MonoBehaviour
{
    [SerializeField] private int maxAmmo;
    [SerializeField] private GameObject shootingUI;
    [SerializeField] private TextMeshProUGUI ammoCountUI;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private Slider shootingSlidingTimerUI;
    [SerializeField] private Image sliderFillImage;  // Reference to the slider's fill image

    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float crosshairScaleDuration = 0.1f;  // Time for the crosshair to scale
    [SerializeField] private float crosshairMaxScale = 1.5f;  // Max scale of the crosshair during shooting effect
    [SerializeField] private float crosshairSpinDuration = 0.5f;  // Time for the crosshair to spin when ammo is given

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Vector3 bulletSpawnPointOffset;  // Where the bullet will spawn
    [SerializeField] private float bulletSpeed = 1000f;   // Speed of the bullet
    [SerializeField] private float bulletLifetime = 5f;

    [SerializeField] private float shootingTime = 10f;
    private float shootingTimeElaspedTime;

    [SerializeField] private float slowMotionScale = 0.2f;   // Time scale for slow motion
    [SerializeField] private float slowMotionTransitionSpeed = 3f;  // Speed to transition to slow motion
    private bool SlowMoActivated = false;

    [SerializeField] private Color slowMotionColor = Color.blue;  // Color for the slider during slow motion
    [SerializeField] private Color normalColor = Color.green;  // Color for the slider in normal mode

    private int currentAmmo;
    private float elaspedTime;
    private Vector3 originalCrosshairScale;  // Store the original crosshair scale

    private void Start()
    {
        currentAmmo = 0;
        UpdateShootingUI();
        originalCrosshairScale = crosshairUI.transform.localScale;  // Store the initial scale
        shootingSlidingTimerUI.maxValue = shootingTime;
        sliderFillImage.color = normalColor;  // Set the slider color to the default normal color
    }

    // Update is called once per frame
    void Update()
    {
        if (elaspedTime > 0f)
        {
            elaspedTime -= Time.deltaTime;
            crosshairUI.GetComponent<Image>().color = Color.black;
        }
        else
        {
            crosshairUI.GetComponent<Image>().color = Color.white;
        }

        if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
        {
            elaspedTime = fireRate;
            --currentAmmo;

            // Create a ray from the center of the camera's viewport
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen (viewport coordinates)

            // Instantiate and fire the bullet in the ray's direction
            FireBullet(ray);

            // Trigger the crosshair scaling effect
            StartCoroutine(ScaleCrosshair());
        }

        // Handle ammo reload
        if (Input.GetKeyUp(KeyCode.R))
        {
            GiveAmmo();
        }

        // Handle slow-motion effect when right mouse button is held
        if (currentAmmo > 0 && Input.GetMouseButton(1))  // Right mouse button held
        {
            ApplySlowMotion();
            SlowMoActivated = true;
        }
        else
        {
            ResetTimeScale();
            SlowMoActivated = false;
        }

        // Update shooting time slider
        if (shootingTimeElaspedTime > 0)
        {
            if (SlowMoActivated)
            {
                shootingTimeElaspedTime -= Time.unscaledDeltaTime * 2;  // Decrease timer twice as fast in slow-motion
            }
            else
            {
                shootingTimeElaspedTime -= Time.unscaledDeltaTime;  // Normal timer decrease
            }
            shootingSlidingTimerUI.value = shootingTimeElaspedTime;
        }
        else
        {
            currentAmmo = 0;
            UpdateShootingUI();
        }
    }

    private void FireBullet(Ray ray)
    {
        // Instantiate the bullet prefab at the spawn point
        GameObject bullet = Instantiate(bulletPrefab, transform.position + bulletSpawnPointOffset, Quaternion.identity);

        // Get the bullet's Rigidbody (assumes the bullet prefab has a Rigidbody)
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        // Apply force to the bullet in the direction of the ray
        bulletRb.AddForce(ray.direction * bulletSpeed);

        // Optionally destroy the bullet after some time to prevent it from existing forever
        Destroy(bullet, bulletLifetime);  // Adjust the time as needed
    }

    private IEnumerator ScaleCrosshair()
    {
        // Scale up the crosshair
        float time = 0f;
        while (time < crosshairScaleDuration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1f, crosshairMaxScale, time / crosshairScaleDuration);
            crosshairUI.transform.localScale = originalCrosshairScale * scale;
            yield return null;
        }

        // Scale back down to the original size
        time = 0f;
        while (time < crosshairScaleDuration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(crosshairMaxScale, 1f, time / crosshairScaleDuration);
            crosshairUI.transform.localScale = originalCrosshairScale * scale;
            yield return null;
        }

        // Ensure the crosshair returns to its original size
        crosshairUI.transform.localScale = originalCrosshairScale;
        UpdateShootingUI();
    }

    private IEnumerator SpinCrosshair()
    {
        // Rotate the crosshair over time
        float elapsed = 0f;
        while (elapsed < crosshairSpinDuration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 360f, elapsed / crosshairSpinDuration); // Full 360-degree spin
            crosshairUI.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        // Reset the crosshair rotation to its original value
        crosshairUI.transform.rotation = Quaternion.identity;
    }

    private void UpdateShootingUI()
    {
        shootingUI.SetActive(currentAmmo > 0);
        ammoCountUI.text = "Ammo: " + currentAmmo;
    }

    public void GiveAmmo()
    {
        currentAmmo = maxAmmo;
        shootingTimeElaspedTime = shootingTime;
        UpdateShootingUI();

        // Start the crosshair animation
        StartCoroutine(ScaleCrosshair());
        StartCoroutine(SpinCrosshair());
    }

    private void ApplySlowMotion()
    {
        // Smoothly transition to slow motion
        Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionScale, Time.deltaTime * slowMotionTransitionSpeed);

        // Maintain a consistent fixedDeltaTime for physics during slow motion
        Time.fixedDeltaTime = 0.02f;  // Keep it constant during slow motion

        // Change the slider color to the slow-motion color
        sliderFillImage.color = slowMotionColor;
    }

    private void ResetTimeScale()
    {
        // Smoothly return to normal time scale
        Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, Time.deltaTime * slowMotionTransitionSpeed);

        // Reset the fixedDeltaTime to the normal value
        Time.fixedDeltaTime = 0.02f;  // Reset to normal physics timestep

        // Change the slider color back to the normal color
        sliderFillImage.color = normalColor;
    }
}
