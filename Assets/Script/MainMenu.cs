using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void Gstart()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void GExit()
    {
        Application.Quit();
    }
    public void Maingo()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
