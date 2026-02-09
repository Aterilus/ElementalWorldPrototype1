using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGate :MonoBehaviour
{
    [Header("Scene to Load")]
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[SceneGate] Trigger entered by: {other.name}");

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[SceneGate] Not Playertag, ignoring.");
            return;
        }

        Debug.Log("Player entered gate, loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
