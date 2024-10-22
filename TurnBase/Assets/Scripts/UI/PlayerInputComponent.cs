using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 负责记录/开启或关闭玩家输入
/// </summary>
public class PlayerInputComponent : MonoBehaviour
{
    public List<Button> buttonList = new List<Button>();

    public RectTransform memoryContainerRect;
    private List<GameObject> memoryList = new List<GameObject>();
    public int MaxActionCount = 5;  // 最大指令数

    // 控制是否输入可用（地图生成前/对战中，所有输入都不可用）
    public static bool InputEnabled = false;
    

    public void UpdateButton(Dictionary<int,int> itemData)
    {
        // 四个物品按钮
        for (int i = 3; i < 7; i++)
        {
            if (itemData[i - 3] <= 0) buttonList[i].interactable = false;
            else buttonList[i].interactable = true;
        }
    }

    public void UpdateCurrMemory(List<int> index)
    {
        index.Sort((a,b)=>b.CompareTo(a));

        foreach (var i in index)
        {
            if (i >= 0 && i < memoryList.Count)
            {
                Destroy(memoryList[i]);
                memoryList.RemoveAt(i);
            }
        }
    }
    
    #region ------按钮事件------

    public void ShowTakeItemPanel()
    {
        GameManagerSingleton.Instance.SetupTakeItemPanel();
    }
    
    public void ShowSwitchPanel()
    {
        GameManagerSingleton.Instance.GamePanel.SetActive(false);
        GameManagerSingleton.Instance.SwitchPanel.SetActive(true);
        GameManagerSingleton.Instance.SetupSwitchPanel();
    }
    
    /// <summary>
    /// 左移
    /// </summary>
    /// <param name="button"></param>
    public void HandleAButton(Button button)
    {
        if (!InputEnabled || memoryList.Count >= MaxActionCount) return;
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.MOVEA);
    }
    
    /// <summary>
    /// 右移
    /// </summary>
    /// <param name="button"></param>
    public void HandleDButton(Button button)
    {
        if (!InputEnabled || memoryList.Count >= MaxActionCount) return;
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.MOVED);
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    /// <param name="button"></param>
    public void HandleJumpButton(Button button)
    {
        if (GameManagerSingleton.Instance.PlayerInputList.Count>0 && GameManagerSingleton.Instance.PlayerInputList[^1] == PlayerInputType.JUMP) return;
        if (!InputEnabled || memoryList.Count >= MaxActionCount) return;
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.JUMP);
    }

    /// <summary>
    /// 横向攻击（轻攻击）
    /// </summary>
    /// <param name="button"></param>
    public void HandleHorizontalAtk(Button button)
    {
        if (!InputEnabled || memoryList.Count >= MaxActionCount) return;
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.ATTACK1);
    }

    /// <summary>
    /// 十字攻击（重攻击）
    /// </summary>
    /// <param name="button"></param>
    public void HandleCrossAtk(Button button)
    {
        if (!InputEnabled || memoryList.Count >= MaxActionCount) return;
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.ATTACK2);
    }

    /// <summary>
    /// 恢复血量
    /// </summary>
    /// <param name="button"></param>
    public void HandleHeal(Button button)
    {
        if (!InputEnabled || memoryList.Count >= MaxActionCount) return;
        GameManagerSingleton.Instance.ItemDic[3]--;
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.HEAL);
        if (GameManagerSingleton.Instance.ItemDic[3] <= 0) button.interactable = false;
    }

    /// <summary>
    /// 护盾
    /// </summary>
    /// <param name="button"></param>
    public void HandleProtected(Button button)
    {
        if (!InputEnabled || memoryList.Count >= MaxActionCount) return;
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.DEFENSE);
    }

    /// <summary>
    /// 进入对战
    /// </summary>
    public void HandleConfirm()
    {
        if (!InputEnabled ) return;
        GameManagerSingleton.Instance.StartBattle();
    }

    #endregion


    /// <summary>
    /// 更新UI
    /// </summary>
    /// <param name="button"></param>
    public void AddBtnToMemoryList(Button button)
    {
        var tmpButton = Instantiate(button, memoryContainerRect);
        tmpButton.interactable = false;
        memoryList.Add(tmpButton.gameObject);
    }

    public void RemoveBtnFromMemoryList(int index)
    {
        Destroy(memoryList[index]);
        memoryList.RemoveAt(index);
    }

    /// <summary>
    /// 清空指令
    /// </summary>
    public void ClearMemoryList()
    {
        foreach (var item in memoryList)
        {
            Destroy(item);
        }
        memoryList.Clear();
        GameManagerSingleton.Instance.PlayerInputList.Clear();
    }
    
    public void HandleClearMemory()
    {
        if(!InputEnabled)return;
        foreach (var item in memoryList)
        {
            Destroy(item);
        }
        memoryList.Clear();
        GameManagerSingleton.Instance.PlayerInputList.Clear();
    }

    // todo fix this logic
    // public void SetBackInputButton(int index)
    // {
    //     if (memoryList.Count < index-1) return;
    //     var btn = memoryList[index];
    //     btn.GetComponent<Image>().color = Color.white;
    // }
    //
    // public void HighlightInputButton(int index)
    // {
    //     if (memoryList.Count < index-1) return;
    //     var btn = memoryList[index];
    //     btn.GetComponent<Image>().color = Color.yellow;
    // }

}
