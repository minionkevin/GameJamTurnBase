using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TakeItemComponent : MonoBehaviour
{
    public RectTransform ItemRect;
    public List<GameObject> NumPrefab = new List<GameObject>();
    public TextMeshProUGUI PasswordLabel;
    public List<GameObject> ItemList = new List<GameObject>();
    public GameObject GamePanel;
    public RectTransform InputRect;
    public GameObject FakeNumPrefab;

    public List<RectTransform> NumPosList = new List<RectTransform>();
    private List<int> inputPassword = new List<int>();
    private StringBuilder currPassword = new StringBuilder();
    private List<GameObject> numButton = new List<GameObject>();
    private List<GameObject> currShowingPassword = new List<GameObject>();

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
            var num = Instantiate(NumPrefab[indices[i]], NumPosList[i]);
            var numComponent = num.GetComponent<NumComponent>();
            numComponent.Setup(indices[i], this);
            numButton.Add(num);
        }

        PasswordLabel.text = "";
    }
    
    // todo 删除用于debug的passwordlabel
    public void AddToPassword(int num, Sprite sprite)
    {
        currPassword.Append(num.ToString());
        inputPassword.Add(num);
        PasswordLabel.text = currPassword.ToString();

        var fakeNum = Instantiate(FakeNumPrefab, InputRect);
        fakeNum.GetComponent<FakeNumComponent>().ChangeSprite(sprite);
        currShowingPassword.Add(fakeNum);
    }

    private void CleanupCurrShowingPassword()
    {
        foreach (var fakeNum in currShowingPassword)
        {
            Destroy(fakeNum);
        }
        currShowingPassword.Clear();
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
            
            // 清空目前输入
            ClearCurrPassword();
        }
    }
    
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        GamePanel.SetActive(true);
        PasswordLabel.text = "";

        foreach (GameObject numButton in numButton)
        {
            Destroy(numButton);
        }
        
        for (int i = ItemRect.childCount - 1; i >= 0; i--)
        {
            Destroy(ItemRect.GetChild(i).gameObject);
        }
        inputPassword.Clear();
        currPassword.Clear();
        numButton.Clear();
        CleanupCurrShowingPassword();
    }

    public void ClearCurrPassword()
    {
        inputPassword.Clear();
        currPassword.Clear();
        PasswordLabel.text = "";
        CleanupCurrShowingPassword();
    }
    
    
}
