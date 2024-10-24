using System.Collections.Generic;
using UnityEngine;

public class PasswordPanelComponent : MonoBehaviour
{
    // 这里只是一个简单的点击按钮后开始
    // 在这个按钮点击之前应该已经把密码对好了

    public List<GameObject> NumPrefab = new List<GameObject>();
    public List<RectTransform> NumPosList = new List<RectTransform>();
    
    private List<GameObject> numButton = new List<GameObject>();

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        List<int> indices = new List<int>();
        
        for (int i = 0; i < 12; i++)
        {
            indices.Add(i);
        }
        System.Random random = new System.Random();
        for (int i = 0; i < indices.Count; i++)
        {
            int randomIndex = random.Next(i, indices.Count);
            (indices[i], indices[randomIndex]) = (indices[randomIndex], indices[i]);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            var num = Instantiate(NumPrefab[indices[i]], NumPosList[i]);
            var numComponent = num.GetComponent<NumComponent>();
            numComponent.PasswordSetup(indices[i]);
            numButton.Add(num);
        }
    }
    
    public void ShowHint1()
    {
        GameManagerSingleton.Instance.FirstHintPanel.SetActive(true);
    }

    public void ShowPasswordPanel()
    {
        GameManagerSingleton.Instance.FirstHintPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void ShowHint2()
    {
        gameObject.SetActive(false);
        GameManagerSingleton.Instance.SecondHintPanel.SetActive(true);
    }
    
    // 隐藏这个按钮直到所有对密码环节完成
    public void HandleStart()
    {
        GameManagerSingleton.Instance.StartGame();
        GameManagerSingleton.Instance.SecondHintPanel.SetActive(false);
    }
}
