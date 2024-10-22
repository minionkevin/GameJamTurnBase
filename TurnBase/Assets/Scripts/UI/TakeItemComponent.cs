using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TakeItemComponent : MonoBehaviour
{
    public RectTransform NumRect;
    public RectTransform ItemRect;
    public GameObject NumPrefab;
    public TextMeshProUGUI PasswordLabel;
    public List<GameObject> ItemList = new List<GameObject>();
    public GameObject GamePanel;
    public List<RectTransform> NumPosList = new List<RectTransform>();

    private List<int> inputPassword = new List<int>();
    private StringBuilder currPassword = new StringBuilder();

    public void Setup()
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
            var num = Instantiate(NumPrefab, NumPosList[i]);
            var numComponent = num.GetComponent<NumComponent>();
            numComponent.Setup(indices[i], this);
        }
        PasswordLabel.text = "";
    }
    
    public void AddToPassword(int num)
    {
        currPassword.Append(num.ToString());
        inputPassword.Add(num);
        PasswordLabel.text = currPassword.ToString();
    }

    public void CheckPassword()
    {
        // placeholder
        string result = string.Join("", inputPassword);
        int currItem;

        switch (result)
        {
            case "0000":
                Instantiate(ItemList[0], ItemRect);
                currItem = 0;
                break;
            case "1111":
                Instantiate(ItemList[1], ItemRect);
                currItem = 1;
                break;
            case "2222":
                Instantiate(ItemList[2], ItemRect);
                currItem = 2;
                break;
            case "3333":
                Instantiate(ItemList[3], ItemRect);
                currItem = 3;
                break;
            default:
                PasswordLabel.text = "WRONG PASSWORD";
                // 在这里播放失败动画
                ClosePanel();
                return;
        }
        
        GameManagerSingleton.Instance.ItemDic[currItem]++;
        GameManagerSingleton.Instance.UpdateAddItems();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        GamePanel.SetActive(true);
        PasswordLabel.text = "";
        for (int i = NumRect.childCount - 1; i >= 0; i--)
        {
            Destroy(NumRect.GetChild(i).gameObject);
        }
        for (int i = ItemRect.childCount - 1; i >= 0; i--)
        {
            Destroy(ItemRect.GetChild(i).gameObject);
        }
        inputPassword.Clear();
        currPassword.Clear();
    }

    public void ClearCurrPassword()
    {
        inputPassword.Clear();
        currPassword.Clear();
        PasswordLabel.text = "";
    }
    
    
}
