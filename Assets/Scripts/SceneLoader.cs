using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour 
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadAfterdelay(string sceneName, float delay) 
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay) 
    {
        Debug.Log($"Waiting for {delay} seconds before loading scene: {sceneName}");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
