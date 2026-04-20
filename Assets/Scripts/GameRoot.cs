using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRoot : MonoBehaviour
{
    public void LoadMainScene()
    {
        SceneManager.LoadScene("SuikaGameScene");
    }
}
