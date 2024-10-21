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
    private int currentAmmo;

    private void Start()
    {
        shootingUI.SetActive(false);
        GiveAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the left mouse button was clicked
        if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
        {
            AmmoLogic();

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

    private void AmmoLogic()
    {
        --currentAmmo;
        if (currentAmmo <= 0)
        {
            shootingUI.SetActive(false);
        }
        else if (ammoCountUI)
        {
            ammoCountUI.text = "Ammo: " + currentAmmo;
        }
    }

    public void GiveAmmo()
    {
        currentAmmo = maxAmmo;
        shootingUI.SetActive(true);
        if (ammoCountUI)
        {
            ammoCountUI.text = "Ammo: " + currentAmmo;
        }
    }
}
