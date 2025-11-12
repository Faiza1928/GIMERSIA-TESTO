using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScripts : MonoBehaviour
{
    public GameObject creditsPanel;

    
    public void OnStartClicked()
    {
        SceneManager.LoadScene("WarungScene"); // Ganti nama scene alur selanjutnya
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Keluar dari game.");
    }


    public void OnCreditsClicked()
    {
        creditsPanel.SetActive(true);
    }

    public void OnCloseCreditsClicked()
    {
        creditsPanel.SetActive(false);
    }
}
