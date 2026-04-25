using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}