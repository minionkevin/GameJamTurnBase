using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchItemComponent : MonoBehaviour
{
    public RectTransform HaveItemRect;
    public RectTransform MissingItemRect;
    public RectTransform PasswordRect;
    public RectTransform ItemStatusRect;
    public GameObject NumPrefab;

    public List<GameObject> ItemList = new List<GameObject>();
    public List<RectTransform> ItemPWStatus = new List<RectTransform>();
    List<int> havepwstatus = new List<int>();

    private GameManagerSingleton GameManager;
    private int deleteId;

    public void Setup()
    {
        for (int i = HaveItemRect.childCount - 1; i >= 0; i--)
        {
            Destroy(HaveItemRect.GetChild(i).gameObject);
        }

        for (int i = MissingItemRect.childCount - 1; i >= 0; i--)
        {
            Destroy(MissingItemRect.GetChild(i).gameObject);
        }
        GameManager = GameManagerSingleton.Instance;
        PasswordRect.gameObject.SetActive(false);
        ShowItems();
        deleteId = -1;
    }


    private void ShowItems()
    {
        foreach (var data in GameManager.ItemDic)
        {
            RectTransform rectTransform;
            GameObject item;

            if (!havepwstatus.Contains(data.Key))
            {
                rectTransform = Instantiate(ItemPWStatus[data.Key], ItemStatusRect);
                havepwstatus.Add(data.Key);
            }


            if (data.Value > 0)
            {
                item = Instantiate(ItemList[data.Key], HaveItemRect);
                item.GetComponent<ItemComponent>().Setup(data.Key, this);
            }
            else
            {
                item = Instantiate(ItemList[data.Key], MissingItemRect);
                item.GetComponent<Button>().interactable = false;
            }

            ShowSendingItemsPW(data.Key);

            item.GetComponent<ItemComponent>().Setup(data.Key,this);

        }

    }
    
    public void SendItem(int id)
    {
        Password.ItemPasswordRenew(id);
        ShowSendingItemsPW(id);

        deleteId = id;
        /*
        StringBuilder sb = new StringBuilder();
        sb.Append(id);
        sb.Append(id);
        sb.Append(id);
        sb.Append(id);
        
        // placeholder password
        foreach (var data in sb.ToString())
        {
            var num = Instantiate(NumPrefab, PasswordRect);
            num.GetComponent<NumComponent>().Setup(data);
        }
        PasswordRect.gameObject.SetActive(true);
        */
    }

    void ShowSendingItemsPW(int id)
    {
        for (int i = 0; i < ItemStatusRect.childCount; i++)
        {
            if (Password.PasswordBook.ContainsKey(i))
            {
                Transform t = ItemStatusRect.GetChild(i);
                t.gameObject.SetActive(true);//Rect
                t.GetChild(0).gameObject.SetActive(true);//child 0ΪImage

                List<int> pw12 = Password.get12PW(i);
                int j = 1;//child 1-4Ϊ����
                foreach (int n in pw12)
                {
                    var num = t.GetComponent<RectTransform>().GetChild(j++).GetComponent<NumComponent>();
                    num.gameObject.SetActive(true);
                    num.Setup(n);
                }
            }
            //    int pwd = Password.PasswordBook[i];
            //    int j = 1;//child 1-4Ϊ����
            //    foreach (char c in pwd.ToString())
            //    {
            //        var num = t.GetComponent<RectTransform>().GetChild(j++).GetComponent<NumComponent>();
            //        num.gameObject.SetActive(true);
            //        num.Setup(c);
            //    }
            //}
            else
                ItemStatusRect.GetChild(i).GetComponent<RectTransform>().gameObject.SetActive(false);
        }
    }

    void clearSendingItemsPW()
    {
        for (int i = ItemStatusRect.childCount - 1; i >= 0; i--)
        {
            Destroy(ItemStatusRect.GetChild(i).gameObject);
        }
        havepwstatus = new List<int>();
    }


    public void UpdateInputAction()
    {
        if(deleteId!=-1)   GameManager.UpdateItems(deleteId);
    }
}
