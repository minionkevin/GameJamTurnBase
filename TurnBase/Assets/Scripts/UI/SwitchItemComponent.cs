using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class SwitchItemComponent : MonoBehaviour
{
    public AudioManager audioManager;
    public RectTransform HaveItemRect;
    public RectTransform MissingItemRect;
    public RectTransform MiddleRect;
    public GameObject Backplate;

    public List<GameObject> ItemList = new List<GameObject>();
    //public List<RectTransform> ItemPWStatus = new List<RectTransform>();
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
            //RectTransform rectTransform;
            GameObject item;

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

            ShowSendedItemsPW(data.Key);

            item.GetComponent<ItemComponent>().Setup(data.Key,this);

        }

    }
    
    public async void SendItem(int id)
    {
        audioManager.AudioUI.clip = audioManager.AudioDic["物品传送成功"];
        audioManager.AudioUI.Play();

        Password.ItemPasswordRenew(id);
        ShowSendingItemsPW(id);

        deleteId = id;

        var sendItem = Instantiate(ItemList[id], MiddleRect);

        Sequence timeline = DOTween.Sequence();
        timeline.Insert(0, sendItem.GetComponent<Image>().DOFade(0, 1.5f));
        timeline.Insert(0.75f, Backplate.transform.DOScale(Vector3.zero, 1.5f));
        timeline.Insert(0.75f, Backplate.transform.DORotate(new Vector3(0, 0 ,360), 1f,RotateMode.FastBeyond360));
        await timeline.Play().AsyncWaitForCompletion();

        UpdateInputAction();
        ClosePanel();
    }

    void ShowSendedItemsPW(int id)
    {
        for (int i = 0; i < MissingItemRect.childCount; i++)
        {
            int itemID = int.Parse(MissingItemRect.GetChild(i).tag);
            if (Password.PasswordBook.ContainsKey(itemID))
            {
                Transform item = MissingItemRect.GetChild(i);
                Transform t = item.GetChild(1);
                t.gameObject.SetActive(true);//Rect

                List<int> pw12 = Password.get12PW(itemID);
                int j = 0;//child 1-5 for password display
                foreach (int n in pw12)
                {
                    var num = t.GetComponent<RectTransform>().GetChild(j++).GetComponent<NumComponent>();
                    num.gameObject.SetActive(true);
                    num.Setup(n);
                    if (j > 5)
                    {
                        //password length out of bounds
                    }
                }
            }
            else
                MissingItemRect.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
    }
    void ShowSendingItemsPW(int id)
    {
        for (int i = 0; i < HaveItemRect.childCount; i++)
        {
            int itemID = int.Parse(HaveItemRect.GetChild(i).tag);
            if (Password.PasswordBook.ContainsKey(itemID))
            {
                Transform item = HaveItemRect.GetChild(i);
                Transform t = item.GetChild(1);
                t.gameObject.SetActive(true);//Rect

                List<int> pw12 = Password.get12PW(itemID);
                int j = 0;//child 1-5 for password display
                foreach (int n in pw12)
                {
                    //var num = t.GetComponent<RectTransform>().GetChild(j++).GetComponent<NumComponent>();
                    var num = t.GetChild(j++).GetComponent<NumComponent>();
                    num.gameObject.SetActive(true);
                    num.Setup(n);
                    if (j > 5)
                    {
                        //password length out of bounds
                    }
                }
            }
            else
                MissingItemRect.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
    }

    void clearSendingItemsPW()
    {
        for (int i = 0; i < MissingItemRect.childCount; i++)
        {
            Destroy(MissingItemRect.GetChild(i).GetChild(1).gameObject);
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
