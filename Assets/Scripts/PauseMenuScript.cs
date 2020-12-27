using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public void PlayButtonFunction()
    {
        GameObject.Find("Road(Generatable)").GetComponent<RoadController>().TogglePause();
    }

    public void MainMenuButtonFunction()
    {
        GameObject.Find("Road(Generatable)").GetComponent<RoadController>().TogglePause();
        SceneManager.LoadScene("MenuScene");
    }
}
