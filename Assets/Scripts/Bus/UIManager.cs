using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private int m_MaxNumOfPassengers;
    private Vehicle m_Bus;

    [Header("In Game UI")]
    [SerializeField] TMP_Text m_PassengerStatusText;
    [SerializeField] TMP_Text m_CurrentNumPassengerText;

    [Header("Win Canvas")]
    [SerializeField] GameObject m_WinCanvas;

    [Header("Pause Canvas")]
    [SerializeField] GameObject m_PauseCanvas;
    [SerializeField] bool m_IsPaused;

    [Header("Lose Canvas")]
    [SerializeField] GameObject m_LoseCanvas;



    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        m_Bus = GameObject.FindWithTag("Player").GetComponent<Vehicle>();

        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Bus == null)
            return;

        PassengerUpdate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    void PassengerUpdate()
    {
        if (m_MaxNumOfPassengers == 0)
        {
            m_MaxNumOfPassengers = GameObject.FindGameObjectsWithTag("Passenger").Length;
        }

        m_PassengerStatusText.text = m_Bus.m_DeliveredPassengers.ToString() + "/" + m_MaxNumOfPassengers.ToString() + " Passengers Delivered";
        m_CurrentNumPassengerText.text = m_Bus.m_Passengers.ToString() + " Passengers On Board";

        if(m_Bus.m_DeliveredPassengers == m_MaxNumOfPassengers)
        {
            m_WinCanvas.SetActive(true);
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Cursor.visible = true;

        }
    }

    public void PauseGame()
    {
        m_IsPaused = !m_IsPaused;
        m_PauseCanvas.SetActive(m_IsPaused);

        if (m_IsPaused)
        {
            Time.timeScale = 0.0f;
            Cursor.visible = true;
        }
        else if (!m_IsPaused)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Cursor.visible = false;
        }
    }

    public void RestartScene()
    {
        int currentScenIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScenIndex);
    }
}