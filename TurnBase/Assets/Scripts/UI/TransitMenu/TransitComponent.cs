using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitComponent : MonoBehaviour
{
    public void HandleAnimationFinish()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
