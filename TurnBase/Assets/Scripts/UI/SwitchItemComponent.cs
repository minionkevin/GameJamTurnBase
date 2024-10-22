using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchItemComponent : MonoBehaviour
{
    public RectTransform HaveItemRect;
    public RectTransform MissingItemRect;
    public RectTransform PasswordRect;
    public GameObject NumPrefab;

    public List<GameObject> ItemList = new List<GameObject>();


    private GameManagerSingleton GameManager;
    private int deleteId;
    
    public void Setup()
    {
            for (int i = HaveItemRect.childCount-1; i >=0 ; i--)
            {
                Destroy(HaveItemRect.GetChild(i).gameObject);
            }            
        
            for (int i = MissingItemRect.childCount-1; i >=0 ; i--)
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
            GameObject item;
            if (data.Value > 0)
            {
                item = Instantiate(ItemList[data.Key], HaveItemRect);
                item.GetComponent<ItemComponent>().Setup(data.Key,this);
            }
            else
            {
                item = Instantiate(ItemList[data.Key], MissingItemRect);
                item.GetComponent<Button>().interactable = false;
            }
            item.GetComponent<ItemComponent>().Setup(data.Key,this);
        }
    }
    
    public void SendItem(int id)
    {
        deleteId = id;
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
    }

    public void UpdateInputAction()
    {
        if(deleteId!=-1)   GameManager.UpdateItems(deleteId);
    }
}
