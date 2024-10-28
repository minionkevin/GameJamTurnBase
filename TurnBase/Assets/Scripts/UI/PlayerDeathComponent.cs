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

    public void ShowStaff()
    {
        gameObject.SetActive(false);
        GameManagerSingleton.Instance.StaffPanel.SetActive(true);
    }
}
