using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    GameObject mainMenu;
    GameObject optionsMenu;
    GameObject helpMenu;
    GameObject quitMenu;

    void Start()
    {
        mainMenu = GameObject.Find("MainButtons");
        optionsMenu = GameObject.Find("OptionsWindow");
        helpMenu = GameObject.Find("HelpWindow");
        quitMenu = GameObject.Find("QuitWindow");

        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        helpMenu.SetActive(false);
        quitMenu.SetActive(false);
    }

    public void PlayPressed()
    {
        Debug.Log("Changed to main scene");
        SceneManager.LoadScene("Main");
    }

    public void ExitPressed()
    {
        Debug.Log("Quit application");
        Application.Quit();
    }
}
