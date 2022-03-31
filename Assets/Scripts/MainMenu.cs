using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject credits;
    public void QuitGame()
    {
        Application.Quit();
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void ShowCredits()
    {
        credits.gameObject.SetActive(true);
    }
}
