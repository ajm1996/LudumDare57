using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string sceneToLoad="SampleScene"; // Assign the name of the scene to load in the inspector

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}