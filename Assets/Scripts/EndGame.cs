using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Menu");
    }

}
