using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// todo ItemStatusRect spawn在item的右边显示密码
// 传送后再次打开没有更新item的位置。传送之后=0的物品依旧在右边。
public class SwitchItemComponent : MonoBehaviour
{
    public RectTransform HaveItemRect;
    public RectTransform MissingItemRect;
    public RectTransform ItemStatusRect;
    public RectTransform MiddleRect;
    public GameObject Backplate;

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
        ResetPanelVisual();
        GameManager = GameManagerSingleton.Instance;
        ShowItems();
        deleteId = -1;
    }

    private void ResetPanelVisual()
    {
        Backplate.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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
    
    public async void SendItem(int id)
    {
        Password.ItemPasswordRenew(id);
        ShowSendingItemsPW(id);

        deleteId = id;

        var sendItem = Instantiate(ItemList[id], MiddleRect);

        Sequence timeline = DOTween.Sequence();
        timeline.Insert(0, sendItem.GetComponent<Image>().DOFade(0, 1.5f));
        timeline.Insert(0.75f, Backplate.transform.DOScale(Vector3.zero, 1.5f));
        timeline.Insert(0.75f, Backplate.transform.DORotate(new Vector3(0, 0 ,360), 1f,RotateMode.FastBeyond360));
        await timeline.Play().AsyncWaitForCompletion();
        
        ClosePanel();
    }

    void ShowSendingItemsPW(int id)
    {
        for (int i = 0; i < ItemStatusRect.childCount; i++)
        {
            if (Password.PasswordBook.ContainsKey(i))
            {
                Transform t = ItemStatusRect.GetChild(i);
                t.gameObject.SetActive(true);//Rect
                t.GetChild(0).gameObject.SetActive(true);//child 0 for Image

                List<int> pw12 = Password.get12PW(i);
                int j = 1;//child 1-5 for password display
                foreach (int n in pw12)
                {
                    var num = t.GetComponent<RectTransform>().GetChild(j++).GetComponent<NumComponent>();
                    num.gameObject.SetActive(true);
                    num.Setup(n);
                    if (j > 6)
                    {
                        //password length out of bounds
                    }
                }
            }
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

    public void ClosePanel()
    {
        GameManagerSingleton.Instance.SendItemPanel(false);
    }


    public void UpdateInputAction()
    {
        if(deleteId!=-1)   GameManager.UpdateItems(deleteId);
    }
}
