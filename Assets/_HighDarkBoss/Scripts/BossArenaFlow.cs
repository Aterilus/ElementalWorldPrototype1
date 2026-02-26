using UnityEngine;
using UnityEngine.SceneManagement;

public class BossArenaFlow : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Scene Names")]
    public string openWorldSceneName = "OpenWorld";
    public string darkArenaSceneName = "DarkBossArena";

    bool ended;

    private void Start()
    {
        if (victoryPanel) { victoryPanel.SetActive(false); }
        if (defeatPanel) { defeatPanel.SetActive(false); }
        ended = false;
        Time.timeScale = 1.0f;
    }

    public void OnBossDefeated()
    {
        if (ended) { return; }
        ended = true;

        if (victoryPanel) { victoryPanel.SetActive(true); }
        Time.timeScale = 0f;
    }

    public void OnPlayerDefeated()
    {
        if (ended) { return; }
        ended = true;

        if (defeatPanel) { defeatPanel.SetActive(true); }
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(darkArenaSceneName);
    }

    public void ReturnToOpenWorld()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(openWorldSceneName);
    }
}
