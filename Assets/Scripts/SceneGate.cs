using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGate :MonoBehaviour
{
    [Header("Scene to Load")]
    public string sceneToLoad = "CombatArea";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Player entered gate, loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
