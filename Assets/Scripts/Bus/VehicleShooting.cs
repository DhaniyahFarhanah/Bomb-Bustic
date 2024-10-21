using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // For sorting with LINQ
using TMPro;

public class VehicleShooting : MonoBehaviour
{
    [SerializeField] private int maxAmmo;
    [SerializeField] private GameObject shootingUI;
    [SerializeField] private TextMeshProUGUI ammoCountUI;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float crosshairScaleDuration = 0.1f;  // Time for the crosshair to scale
    [SerializeField] private float crosshairMaxScale = 1.5f;  // Max scale of the crosshair during shooting effect
    private int currentAmmo;
    private float elaspedTime;
    private Vector3 originalCrosshairScale;  // Store the original crosshair scale

    private void Start()
    {
        shootingUI.SetActive(false);
        originalCrosshairScale = crosshairUI.transform.localScale;  // Store the initial scale
        GiveAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        if (elaspedTime > 0f)
        {
            elaspedTime -= Time.deltaTime;
        }
        else if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
        {
            elaspedTime = fireRate;
            --currentAmmo;

            // Create a ray from the center of the camera's viewport
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen (viewport coordinates)

            // Raycast all objects along the ray
            RaycastHit[] hits = Physics.RaycastAll(ray);

            // Check if we hit anything
            if (hits.Length > 0)
            {
                // Convert the array to a list so we can sort it
                List<RaycastHit> hitList = hits.ToList();

                // Sort by distance (smallest distance first)
                hitList.Sort((hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

                CheckHitList(hitList);
            }

            // Trigger the crosshair scaling effect
            StartCoroutine(ScaleCrosshair());
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            GiveAmmo();
        }
    }

    private void CheckHitList(List<RaycastHit> hitObjects)
    {
        // Loop through the sorted hit objects
        foreach (RaycastHit hit in hitObjects)
        {
            GameObject clickedObject = hit.collider.gameObject;
            if (clickedObject)
            {
                //Debug.Log("Hit: " + clickedObject.name + " at distance: " + hit.distance);
                if (clickedObject.GetComponentInParent<RougeAI>())
                {
                    clickedObject.GetComponentInParent<RougeAI>().SelfDestruct();
                    return;
                }

                //If object has an ObstacleType it means that it should block the raycast
                if (clickedObject.GetComponent<ObstacleType>())
                    return;
            }
        }
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

    private void UpdateShootingUI()
    {
        shootingUI.SetActive(currentAmmo > 0);
        if (ammoCountUI)
        {
            ammoCountUI.text = "Ammo: " + currentAmmo;
        }
    }

    public void GiveAmmo()
    {
        currentAmmo = maxAmmo;
        UpdateShootingUI();
    }
}
