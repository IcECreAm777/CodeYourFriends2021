using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Makes the start button load a scene - change the "SampleScene" to the actual game Scene in the future. Also in build settings make sure we ad the scene we want to load.
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    //go to the actual game from tutorial
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("SnappingTestScene");
    }
}
