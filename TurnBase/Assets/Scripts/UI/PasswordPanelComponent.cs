using UnityEngine;

public class PasswordPanelComponent : MonoBehaviour
{
    // 这里只是一个简单的点击按钮后开始
    // 在这个按钮点击之前应该已经把密码对好了

    public void HandleAwake()
    {
        gameObject.SetActive(true);
    }

    // 隐藏这个按钮直到所有对密码环节完成
    public void HandleStart()
    {
        GameManagerSingleton.Instance.StartGame();
        gameObject.SetActive(false);
    }
}
