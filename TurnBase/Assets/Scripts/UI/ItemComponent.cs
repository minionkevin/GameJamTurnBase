using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemComponent : MonoBehaviour
{
    public TextMeshProUGUI ItemLabel;
    [HideInInspector]
    public int Id;

    private SwitchItemComponent switchPanel;
    
    public void Setup(int id,SwitchItemComponent switchPanel)
    {
        Id = id;
        this.switchPanel = switchPanel;
    }

    public void CheckItem()
    {
        // 更新manager
        GameManagerSingleton.Instance.ItemDic[Id]--;
        if (Id == 3) GameManagerSingleton.Instance.ItemDic[Id] = 0;

        if (GameManagerSingleton.Instance.ItemDic[Id] == 0)
        {
            GetComponent<Button>().interactable = false;
            if (Id == 3) GameManagerSingleton.Instance.HealCountLabel.text = "0";
        }
        switchPanel.SendItem(Id);
    }
}
