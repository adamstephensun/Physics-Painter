using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameObject pauseMenu;
    private GameObject quitConfirm;
    private GameObject resetConfirm;

    private OSCSender oscTransmitter;

    [SerializeField] private shoot shootController;
    [SerializeField] private FirstPersonLook lookController;

    public Image darkImage;
    public Image lightImage;
    [SerializeField] private GameObject notifText;
    [SerializeField] AudioManager audioManager;

    private Sprite moonSprite;
    private Sprite sunSprite;

    private bool paused;
    private bool darkMode;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        quitConfirm = GameObject.Find("QuitConfirm");
        resetConfirm = GameObject.Find("ResetConfirm");

        oscTransmitter = GameObject.Find("OSC Sender").GetComponent<OSCSender>();

        pauseMenu.SetActive(false);     //Hide all menus on startup
        quitConfirm.SetActive(false);
        resetConfirm.SetActive(false);
        notifText.SetActive(false);

        paused = false;
        darkMode = true;

        if (darkMode)
        {
            darkImage.enabled = true;
            lightImage.enabled = false;
        }
        else
        {
            darkImage.enabled = false;
            lightImage.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                audioManager.playSound("MenuForward");
                pause();
            }
            else
            {
                unpause();
                audioManager.playSound("MenuBack");
            }
        }
    }

    public void pause()     //pause the game
    {
        pauseMenu.SetActive(true);  //display pause menu
        paused = true;  //set flag

        Time.timeScale = 0;                 //Stop time
        shootController.enabled = false;    //Disable the shoot function
        lookController.enabled = false;     //Disable player movement
        Cursor.lockState = CursorLockMode.None; //Unlock cursor
        Cursor.visible = true;

        quitConfirm.SetActive(false);
        resetConfirm.SetActive(false);

        Debug.Log("Game paused");
    }

    public void unpause()       //Unpause the game
    {
        pauseMenu.SetActive(false);
        notifText.SetActive(false);
        paused = false;

        Time.timeScale = 1;
        shootController.enabled = true;
        lookController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Game unpaused");
    }

    public void exitGame()      //Return to main menu
    {
        unpause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    public void resetGame()     //Reset the scene
    {
        oscTransmitter.resetScene();    //Send osc message to reset processing scene
        unpause();
        SceneManager.LoadScene("Main");
    }


    public void saveCopy()      //Displays the text notification for a new copy of the canvas being saved
    {
        notifText.GetComponent<TextMeshProUGUI>().SetText("Canvas copy saved as savedCanvas"+oscTransmitter.getSaveIndex()+".png");
        StartCoroutine(displayNotifText());
    }

    public void toggleDarkmode()
    {
        oscTransmitter.toggleDarkMode();
        darkMode = !darkMode;
        Debug.Log("Darkmode toggled to:" + darkMode);

        if (darkMode){
            darkImage.enabled = true;
            lightImage.enabled = false;

            notifText.GetComponent<TextMeshProUGUI>().SetText("Dark mode enabled on reset.");
        }
        else{
            darkImage.enabled = false;
            lightImage.enabled = true;

            notifText.GetComponent<TextMeshProUGUI>().SetText("Light mode enabled on reset.");
        }
        StartCoroutine(displayNotifText());
    }

    IEnumerator displayNotifText()
    {
        notifText.SetActive(true);
        yield return new WaitForSecondsRealtime(3.0f);
        notifText.SetActive(false);
    }
}
