using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] bool m_HideMouseOnStart;
    [SerializeField] bool m_FindBus;
    private int m_MaxNumOfPassengers;
    private Vehicle m_Bus;

    [Header("In Game UI")]
    [SerializeField] TMP_Text m_PassengerStatusText;
    [SerializeField] TMP_Text m_CurrentNumPassengerText;

    [Header("Win Canvas")]
    public bool end, win, playOnce;
    [SerializeField] BusPassengers m_PassengerInfo;
    [SerializeField] GameObject m_PassengerIconPrefab;
    [SerializeField] GameObject m_PassengerShowcase;
    [SerializeField] float m_Time;
    [SerializeField] GameObject m_WinCanvas;
    [SerializeField] Color m_DeliveredColor;
    [SerializeField] Color m_InjuredColor;
    [SerializeField] Color m_LostColor;
    [SerializeField] Color m_unsavedColor;
    [SerializeField] private Animator m_WinCanvasAnim;
    [SerializeField] private float score;
    [SerializeField] private TMP_Text timeTextBox;
    [SerializeField] private TMP_Text gradeTextBox;
    [SerializeField] Color bronzeColor;
    [SerializeField] Color silverColor;
    [SerializeField] Color goldColor;
    [SerializeField] private float silverScore;
    [SerializeField] private float goldScore;

    [Header("Pause Canvas")]
    [SerializeField] GameObject m_PauseCanvas;
    public bool m_IsPaused;

    [Header("Lose Canvas")]
    [SerializeField] GameObject m_LoseCanvas;
    [SerializeField] private TMP_Text timeTextBoxLose;
    [SerializeField] private TMP_Text passengersSavedText;
    [SerializeField] private GameObject loseHolder;
    private int savedPassengers;

    private AudioSource _AudioSource;
    private bool winOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        playOnce = false;
        end = false;
        win = false;

        m_Time = 0f;

        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (m_FindBus)
        {
            m_Bus = GameObject.FindWithTag("Player").GetComponent<Vehicle>();
            m_PassengerInfo = m_Bus.GetComponent<BusPassengers>();
        }

        if (m_HideMouseOnStart)
        {
            Cursor.visible = false;
        }
        
        _AudioSource = gameObject.AddComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Bus == null)
            return;

        //PassengerUpdate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !end)
        {
            PauseGame();
        }

        if (!end)
        {
            m_Time += Time.deltaTime;
        }

        if(end && !win && Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    void CalculateScore()
    {
        score = score - m_Time;
        timeTextBox.text = m_Time.ToString("00:00");

        for (int i = 0; i < m_PassengerInfo.PassengerStateList.Count; i++)
        {
            switch (m_PassengerInfo.PassengerStateList[i])
            {
                case PassengerState.injured: score -= 1000; break;
                case PassengerState.saved: score += 1000; break;
                case PassengerState.lost: score += 500; break;
            }
        }

        GiveGrade();

    }
    void GiveGrade()
    {
        if(score > 0 && score < silverScore)
        {
            gradeTextBox.text = "BRONZE";
            gradeTextBox.color = bronzeColor;
        }
        else if(score > silverScore && score < goldScore)
        {
            gradeTextBox.text = "SILVER";
            gradeTextBox.color = silverColor;
        }
        else if(score > goldScore)
        {
            gradeTextBox.text = "GOLD";
            gradeTextBox.color = goldColor;
        }
        else
        {
            gradeTextBox.text = "RIP";
            gradeTextBox.color = Color.red;
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
            if(!winOnce)
            {
                Play(m_Bus.GetComponent<BusAudioHandler>().win);
                winOnce = true;
            }
            
            m_WinCanvas.SetActive(true);
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            if (m_HideMouseOnStart)
            {
                Cursor.visible = false;
            }

        }
    }

    public void PauseGame()
    {
        if (!end)
        {
            if (m_PauseCanvas == null)
            {
                return;
            }

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
                if (m_HideMouseOnStart)
                {
                    Cursor.visible = false;
                }
            }
        }
    }

    public void RestartScene()
    {
        int currentScenIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScenIndex);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void LoadNextScene()
    {
        int currentScenIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadChosenSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadChosenSceneByNumber(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Play(AudioClip clip) {
        _AudioSource.clip = clip;
        _AudioSource.Play();
    }

    public void Win()
    {
        win = true;
        end = true;
        m_WinCanvas.SetActive(true);
        Cursor.visible = true;

        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        CalculateScore();
    }

    public void Lose()
    {
        win = false;
        end = true;
        m_LoseCanvas.SetActive(true);
        Cursor.visible = true;

        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        timeTextBoxLose.text = m_Time.ToString("00:00");

        for (int i = 0; i < m_PassengerInfo.PassengerStateList.Count; i++)
        {
            Image icon = Instantiate(m_PassengerIconPrefab, m_PassengerShowcase.transform).GetComponent<Image>();

            switch (m_PassengerInfo.PassengerStateList[i])
            {
                case PassengerState.saved: savedPassengers++; break;
                case PassengerState.injured: savedPassengers++; break;
            }
        }

        passengersSavedText.text = savedPassengers.ToString() + "/" + m_PassengerInfo.PassengerStateList.Count;


    }

    public void ShowPassengers()
    {
        for(int i = 0; i < m_PassengerInfo.PassengerStateList.Count; i++)
        {
            Image icon = Instantiate(m_PassengerIconPrefab, m_PassengerShowcase.transform).GetComponent<Image>();

            switch (m_PassengerInfo.PassengerStateList[i])
            {
                case PassengerState.injured: icon.color = m_InjuredColor; break;
                case PassengerState.saved: icon.color = m_DeliveredColor; break;
                case PassengerState.lost: icon.color = m_LostColor; break;
                default : icon.color = Color.grey; break;
            }
        }
    }

    public void ShowPassengersLose()
    {
        for (int i = 0; i < m_PassengerInfo.PassengerStateList.Count; i++)
        {
            Image icon = Instantiate(m_PassengerIconPrefab, loseHolder.transform).GetComponent<Image>();

            switch (m_PassengerInfo.PassengerStateList[i])
            {
                case PassengerState.injured: icon.color = m_InjuredColor; break;
                case PassengerState.saved: icon.color = m_DeliveredColor; break;
                case PassengerState.lost: icon.color = m_LostColor; break;
                default: icon.color = Color.grey; break;
            }
        }
    }

    public IEnumerator InstantiatePassengers(float timeper)
    {
        for (int i = 0; i < m_PassengerInfo.PassengerStateList.Count; i++)
        {
            Image icon = Instantiate(m_PassengerIconPrefab, m_PassengerShowcase.transform).GetComponent<Image>();

            switch (m_PassengerInfo.PassengerStateList[i])
            {
                case PassengerState.injured: icon.color = m_InjuredColor; break;
                case PassengerState.saved: icon.color = m_DeliveredColor; break;
                case PassengerState.lost: icon.color = m_LostColor; break;
                default: icon.color = Color.grey; break;
            }

            yield return new WaitForSecondsRealtime(timeper / m_PassengerInfo.PassengerStateList.Count);
        }
    }

    public IEnumerator InstantiatePassengersLose(float timeper)
    {
        for (int i = 0; i < m_PassengerInfo.PassengerStateList.Count; i++)
        {
            Image icon = Instantiate(m_PassengerIconPrefab, loseHolder.transform).GetComponent<Image>();

            switch (m_PassengerInfo.PassengerStateList[i])
            {
                case PassengerState.injured: icon.color = m_InjuredColor; break;
                case PassengerState.saved: icon.color = m_DeliveredColor; break;
                case PassengerState.lost: icon.color = m_LostColor; break;
                case PassengerState.undefined: icon.color = m_unsavedColor; break;
                default: icon.color = Color.grey; break;
            }

            yield return new WaitForSecondsRealtime(timeper / m_PassengerInfo.PassengerStateList.Count);
        }
    }
}
