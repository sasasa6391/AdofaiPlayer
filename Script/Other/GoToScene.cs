using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    public void StartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
