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
        int currItem=-1;
        int itemIndex = -1;
        int inputPW = Password.To10(inputPassword);
        //for (int i = inputPassword.Count; i > 0; i--)
        //{ 
        //    inputPW += int.Parse((inputPassword[inputPassword.Count-i] *1.0 * Math.Pow(10, i-1)).ToString());
        //}
        if (Password.pwdVerified(inputPW))
        {
            foreach (int itemid in Password.ItemMark)
            {
                itemIndex++;

                if (inputPW % itemid == 0)
                {
                    currItem = itemIndex;
                    Instantiate(ItemList[currItem], ItemRect);
                    //获取物品成功，则检查传出状态，如果是之前传出的物品，此时更新状态取消密码显示
                    if(Password.PasswordBook.ContainsKey(currItem))
                    {
                        Password.PasswordBook.Remove(currItem);
                    }
                    break;
                }
            }
            GameManagerSingleton.Instance.ItemDic[currItem]++;
            GameManagerSingleton.Instance.UpdateAddItems();
        }
        else
        {
            // 在这里播放失败动画
            ClosePanel();
        }

        /*
        // placeholder
        string result = string.Join("", inputPassword);


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
        */

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
