using System;
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

    public void HandleAButton(Button button)
    {
        HandlePlayerMove(new int2(-1, 0), button);
    }
    
    public void HandleDButton(Button button)
    {
        HandlePlayerMove(new int2(1, 0), button);
    }
    
    public void HandleSButton(Button button)
    {
        HandlePlayerMove(new int2(0, -1), button);
    }
    
    public void HandleWButton(Button button)
    {
        HandlePlayerMove(new int2(0, 1), button);
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
        TriggerPlayerInput?.Invoke();

        // confirm之后需要block玩家输入
        // 直到下一轮到玩家
        
        // 这里应该有个callback
        // 当这件事完成之后，销毁这个，再去完成下一个，然后再销毁
        foreach (var item in memoryList)
        {
            Destroy(item);
        }
    }
}
