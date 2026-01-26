using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerKiller : MonoBehaviour
{
    [SerializeField] private float deathDelay = 0.5f;  // Optional: time before reload (for death anim/sound)
    [SerializeField] private string mainMenuScene = "MainMenu";  // Drag or type scene name

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Killer hit: {other.name} (Tag: {other.tag})");  // DEBUG

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Not Player - ignoring");
            return;
        }

        Debug.Log("PLAYER KILLED! Reloading...");

        // Optional: Disable player movement/collider here
        // other.GetComponent<PlayerController>().enabled = false;  // If you have a controller script

        // Reload after delay (play death sound/anim first)
        Invoke(nameof(ReloadToMenu), deathDelay);
    }

    private void ReloadToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}