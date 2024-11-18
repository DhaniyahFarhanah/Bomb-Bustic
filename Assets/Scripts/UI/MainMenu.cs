using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject instructionPage;
    bool instructionReady;
    bool canStart;
    [SerializeField] float buttonDisabledTime;
    [SerializeField] GameObject[] pages;
    [SerializeField] TMP_Text tilNextText;
    [SerializeField] Image nextButton;
    [SerializeField] Color disabledButtonColor;
    [SerializeField] Color enabledButtonColor;
    private float timer;

    private int PageIndex = 0;
    private UIManager UIManager;

    // Start is called before the first frame update
    void Start()
    {
        PageIndex = 0;
        timer = buttonDisabledTime;
        canStart = false;
        instructionReady = false;
        UIManager = GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (instructionReady)
        {
            if(timer >= 0)
            {
                timer-= Time.deltaTime;
                tilNextText.text = ((int)timer + 1).ToString() + "s";
                nextButton.color = disabledButtonColor;
            }

            else if(timer < 0)
            {
                if(PageIndex >= pages.Length -1)
                {
                    tilNextText.text = "Play";
                    canStart = true;
                }
                else
                {
                    tilNextText.text = "Next";
                }

                nextButton.color = enabledButtonColor;
            }
        }
    }

    public void OpenInstructionPage()
    {
        mainMenu.SetActive(false);
        instructionPage.SetActive(true);
        instructionReady = true;

        for (int i = 0; i < pages.Length; i++)
        {
            if (i == PageIndex)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }

    }

    public void GoToNextPage()
    {
        if(timer <= 0)
        {
            if (canStart)
            {
                UIManager.LoadChosenSceneByName("Level");
            }
            else
            {
                timer = buttonDisabledTime;
                PageIndex++;

                for (int i = 0; i < pages.Length; i++)
                {
                    if (i == PageIndex)
                    {
                        pages[i].SetActive(true);
                    }
                    else
                    {
                        pages[i].SetActive(false);
                    }
                }
            }
            
        }
    }



}
