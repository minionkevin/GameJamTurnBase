using UnityEngine;

public class PlayerDeathComponent : MonoBehaviour
{
    public void RestartGame()
    {
        GameManagerSingleton.Instance.Restart();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
