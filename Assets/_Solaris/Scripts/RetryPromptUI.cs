using UnityEngine;

public class RetryPromptUI : MonoBehaviour
{
    public GameObject panel;
    public SolarisController solaris;

    public void Show()
    { 
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRetryButtonPressed()
    {
        Hide();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void OnNotNow()
    {
        Hide();
        Debug.Log("Player chose not now.");
    }
}
