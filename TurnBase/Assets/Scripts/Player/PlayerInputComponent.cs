using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputComponent : MonoBehaviour
{
    public List<Button> buttonList = new List<Button>();
    public RectTransform memoryContainerRect;
    
    private Action TriggerPlayerInput;
    private List<GameObject> memoryList = new List<GameObject>();


    public Action OnBattleStart;
    public Action OnBattleEnd;

    // 控制是否输入可用（当对战时，所有输入都不可用）
    bool InputEnabled = true;

    private void OnEnable()
    {
        CountDown.Instance.OnTimerEnd += EnterBattle;
        OnBattleEnd += CountDown.Instance.SetTimerInit;
    }

    private void OnDisable()
    {
        CountDown.Instance.OnTimerEnd -= EnterBattle;
        OnBattleEnd -= CountDown.Instance.SetTimerInit;
    }

    public void HandleAButton(Button button)
    {
        if (!InputEnabled)
            return;
        HandlePlayerMove(new int2(-1, 0), button);
    }
    
    public void HandleDButton(Button button)
    {
        if (!InputEnabled)
            return;
        HandlePlayerMove(new int2(1, 0), button);
    }
    
    public void HandleSButton(Button button)
    {
        if (!InputEnabled)
            return;
        HandlePlayerMove(new int2(0, -1), button);
    }
    
    public void HandleWButton(Button button)
    {
        if (!InputEnabled)
            return;
        HandlePlayerMove(new int2(0, 1), button);
    }

    /// <summary>
    /// 横向攻击（轻攻击）
    /// </summary>
    /// <param name="button"></param>
    public void HandleHorizontalAtk(Button button)
    {
        
    }

    /// <summary>
    /// 十字攻击（重攻击）
    /// </summary>
    /// <param name="button"></param>
    public void HandleCrossAtk(Button button)
    {
        
    }

    private void HandlePlayerAtk(List<int2> atkPos, Button button)
    {
        
    }

    private void HandlePlayerMove(int2 pos,Button button)
    {
        var tmpButton = Instantiate(button, memoryContainerRect);
        tmpButton.interactable = false;
        TriggerPlayerInput += () => GameManagerSingleton.Instance.Player.HandleMove(pos);
        memoryList.Add(tmpButton.gameObject);
    }


    public void HandleConfirm()
    {
        EnterBattle();
    }

    /// <summary>
    /// 进入对战（计时结束或者直接确认）
    /// </summary>
    public void EnterBattle()
    {
        // 禁用一切输入

        // 计时器暂停（等到下一回合重置）
        CountDown.Instance.SetTimerPause();
        // 对战
        StartCoroutine (BattleCoroutine());
    }

    /// <summary>
    /// 对战过程
    /// </summary>
    /// <returns></returns>
    public IEnumerator BattleCoroutine()
    {
        OnBattleStart?.Invoke();

        TriggerPlayerInput?.Invoke();
        TriggerPlayerInput = null;

        // confirm之后需要block玩家输入
        // 直到下一轮到玩家
        yield return new WaitForFixedUpdate();
        // 这里应该有个callback
        // 当这件事完成之后，销毁这个，再去完成下一个，然后再销毁
        foreach (var item in memoryList)
        {
            Destroy(item);
        }

        OnBattleEnd?.Invoke();
        yield return null;
    }

}
