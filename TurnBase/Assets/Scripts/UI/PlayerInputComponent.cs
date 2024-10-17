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

    #region ------按钮事件------

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
        if (GameManagerSingleton.Instance.PlayerInputList[^1] == PlayerInputType.JUMP) return;
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
        AddBtnToMemoryList(button);
        GameManagerSingleton.Instance.PlayerInputList.Add(PlayerInputType.HEAL);
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

    public void HandleSwitchItem()
    {
        if (!InputEnabled) return;
        
        // 1.打开新ui界面
        // 2.生成 拥有/未拥有物品
        // 3.拥有物品可以传送
        // 4.密码？
        // 5.取出=指令=
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

    // --------------------------------------------------------------

    /// <summary>
    /// 更新UI
    /// </summary>
    /// <param name="button"></param>
    private void AddBtnToMemoryList(Button button)
    {
        var tmpButton = Instantiate(button, memoryContainerRect);
        tmpButton.interactable = false;
        memoryList.Add(tmpButton.gameObject);
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
    }

    public void SetBackInputButton(int index)
    {
        var btn = memoryList[index];
        btn.GetComponent<Image>().color = Color.white;
    }

    public void HighlightInputButton(int index)
    {
        var btn = memoryList[index];
        btn.GetComponent<Image>().color = Color.yellow;
    }


    // -----------------------------------------------------------

    [Obsolete]
    private void HandlePlayerMove(int2 dir,Button button)
    {
        //// UI显示更新
        //var tmpButton = Instantiate(button, memoryContainerRect);
        //tmpButton.interactable = false;
        //memoryList.Add(tmpButton.gameObject);
        //// 跟新事件列表
        //GameManagerSingleton.Instance.PlayerActionList += () => GameManagerSingleton.Instance.Player.HandleMove(dir);
    }

    [Obsolete]
    private void HandlePlayerAttack(List<int2> posList, Button button)
    {
        //// UI显示更新
        //var tmpButton = Instantiate(button, memoryContainerRect);
        //tmpButton.interactable = false;
        //memoryList.Add(tmpButton.gameObject);
        //// 跟新事件列表
        //GameManagerSingleton.Instance.PlayerActionList += () => GameManagerSingleton.Instance.Player.HandleAttack(posList);
    }

    // 无上下移动
    [Obsolete]
    public void HandleSButton(Button button)
    {
        if (!InputEnabled) return;
        HandlePlayerMove(new int2(0, -1), button);
    }

    [Obsolete]
    public void HandleWButton(Button button)
    {
        if (!InputEnabled) return;
        HandlePlayerMove(new int2(0, 1), button);
    }


}
