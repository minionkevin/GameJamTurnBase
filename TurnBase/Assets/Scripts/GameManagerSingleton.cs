using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Serialization;

public class GameManagerSingleton : BaseSingleton<GameManagerSingleton>
{
    // 这里的大部分数据应该都是private+从data里read
    // 写成public只是为了测试方便
    public int Width = 7;
    public int Height = 6;

    public BossActionScriptableObject BossData;
    public ItemScriptableObject AItemData;
    public ItemScriptableObject BItemData;
    public BossInputScriptableObject ABossInputData;
    public BossInputScriptableObject BBossInputData;

    public int2 bossHeadPos;
    public int2 bossLeftHandPos;
    public int2 bossRightHandPos;

    public int bossStartHp = 40;
    public int playerStartHp = 4;       // 玩家初始血量（心形的个数）
    public int countdownTime = 40;      
    
    // 有了config之后从scriptableobject read
    public int2 PlayerStartPos;
    
    public GameObject APlayerPrefab;
    public GameObject BPlayerPrefab;

    public GameObject BossHeadPrefab;
    public GameObject BossLeftHandPrefab;
    public GameObject BossRightHandPrefab;
    public Animator BossAnimator;
    public Animator PlayerAnimator;

    public CountDown CountDown_UI;
    public BossHp BossHp_UI;
    public PlayerHp PlayerHp_UI;

    public GameObject GamePanel;
    public GameObject SwitchPanel;
    public GameObject TakeItemPanel;
    public GameObject PlayerDeathPanel;

    public PlayerComponent Player;
    public BossComponent Boss;
    
    public List<int> PlayerInputList = new List<int>();
    public List<int> BossInputList = new List<int>();

    public List<int> inputLists = new List<int>();
    
    public PlayerInputComponent PlayerInput;

    public Dictionary<int, int> ItemDic = new Dictionary<int, int>();

    public bool IsPlayerA;
    public bool IsPlayerDie;

    public TextMeshProUGUI CurrTurnLabel;
    
    private int currTurnNum;
    private List<int> BossActionList = new List<int>();


    void Start()
    {
        // Setup
        currTurnNum = 0;
        IsPlayerA = PlayerData.IsPlayerA;
        // todo change this back
        IsPlayerA = true;
        TileManagerSingleton.Instance.Setup(Width, Height);
        
        // Player spawn
        var player = Instantiate(IsPlayerA? APlayerPrefab:BPlayerPrefab);
        var playerComponent = player.GetComponent<PlayerComponent>();
        Player = playerComponent;
        playerComponent.Setup(PlayerStartPos);
        TileManagerSingleton.Instance.AddObjectToTile(PlayerStartPos,player);

        // Boss spawn
        // 这里的boss只是一个数据载体，剩余的三个boss prefab才是视觉上看到的
        var bossHead = Instantiate(BossHeadPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossHeadPos,bossHead);
        var bossComponent = bossHead.GetComponent<BossComponent>();
        Boss = bossComponent;
        
        // Boss hand spawn
        var bossLeftHand = Instantiate(BossLeftHandPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossLeftHandPos,bossLeftHand);
        var bossRightHand = Instantiate(BossRightHandPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossRightHandPos,bossRightHand);
        bossComponent.Setup(bossHeadPos,bossLeftHandPos,bossRightHandPos,bossHead,bossLeftHand,bossRightHand);

        // Animator
        BossAnimator = bossHead.GetComponent<Animator>();
        PlayerAnimator = player.GetComponent<Animator>();

        foreach (var data in IsPlayerA ? ABossInputData.InputList : BBossInputData.InputList)
        {
            BossActionList.Add(data);
        }
        
        // UI spawn
        BossHp_UI.Setup(bossStartHp);
        PlayerHp_UI.Setup(playerStartHp);
        
        // Start game
        HandlePlayerTurn();
        SetupItems();
        StartCoroutine(BattleCoroutine());
        
        CurrTurnLabel.text = currTurnNum.ToString();
    }

    private void SetupItems()
    {
        // 更新dic,本身的数据就不重要了
        foreach (var item in IsPlayerA ? AItemData.ItemDatas: BItemData.ItemDatas)
        {
            ItemDic.Add(item.Id,item.Amount);
        }
        PlayerInput.UpdateButton(ItemDic);
    }

    public void SetupTakeItemPanel()
    {
        TakeItemPanel.GetComponent<TakeItemComponent>().Setup();
    }
    
    public void UpdateAddItems()
    {
        PlayerInput.UpdateButton(ItemDic);
        
        // 取东西成功添加空指令占位置
        if (PlayerInputList.Count == 5)
        {
            PlayerInputList.RemoveAt(4);
            PlayerInput.RemoveBtnFromMemoryList(4);
        }
        PlayerInput.AddBtnToMemoryList(PlayerInput.buttonList[7]);
        if (PlayerInputList.Count > 0) PlayerInputList[^1] = -1;
        else PlayerInputList.Add(-1);
    }

    public void UpdateItems(int deleteID)
    {
        PlayerInput.UpdateButton(ItemDic);
        
        // 如果没有到0就不用考虑删指令
        if(ItemDic[deleteID] >0) return;

        int deleteType = -1;
        switch (deleteID)
        {
            case 0:
                deleteType = PlayerInputType.ATTACK1;
                break;
            case 1:
                deleteType = PlayerInputType.ATTACK2;
                break;
            case 2:
                deleteType = PlayerInputType.DEFENSE;
                break;
            case 3:
                deleteType = PlayerInputType.HEAL;
                break;
        }
        
        if(deleteType==-1) return;
        
        List<int> itemsToRemove = new List<int>();
        List<int> memoryRemoveList = new List<int>();
        for (int i = 0; i < PlayerInputList.Count; i++)
        {
            if (PlayerInputList[i] == deleteType)
            {
                itemsToRemove.Add(PlayerInputList[i]);
                memoryRemoveList.Add(i);
            }
        }
        PlayerInput.UpdateCurrMemory(memoryRemoveList);
        
        foreach (var item in itemsToRemove)
        {
            PlayerInputList.Remove(item);
        }

    }

    public void SetupSwitchPanel()
    {
        SwitchPanel.GetComponent<SwitchItemComponent>().Setup();
    }

    
    // todo timer check
    /// <summary>
    /// 开启游戏计时，启用玩家输入，正式进入游戏
    /// </summary>
    private void HandlePlayerTurn()
    {
        // 开启游戏计时
        CountDown_UI.Setup(countdownTime);
        // 启用玩家输入
        PlayerInputComponent.InputEnabled = true;
        
        // todo 检测玩家血量并决定下一个boss指令
        HandleBossActions();
    }

    /// <summary>
    /// 进入对战（计时结束或者直接确认）
    /// </summary>
    public void StartBattle()
    {
        // 禁用一切输入
        PlayerInputComponent.InputEnabled = false;
        // 计时器暂停（等到下一回合重置）
        CountDown.Instance.SetTimerPause();
        
        // 进入对战
        StartCoroutine(BattleCoroutine());
    }

    private void HandleBossActions()
    {
        for (int i = 0; i < BossData.BossActions[BossActionList[currTurnNum]].BossActions.Count; i++)
        {
            if (currTurnNum != 0 && i == 0) continue;
            BossInputList.Add(BossData.BossActions[BossActionList[currTurnNum]].BossActions[i]);
        }  
    }
    
    // todo call this after boss start animation finish
    private void BossStartPoss()
    {
        inputLists.Add(BossData.BossActions[BossActionList[0]].BossActions[0]);
    }

    /// <summary>
    /// 交替重排指令
    /// </summary>
    private void ReorderInput()
    {
        while(PlayerInputList.Count < 6) PlayerInputList.Add(-1);
        for (int i = 0; i < Math.Max(BossInputList.Count, PlayerInputList.Count) * 2; i++)
        {
            if(i < PlayerInputList.Count) inputLists.Add(PlayerInputList[i]);
            if (i < BossInputList.Count)  inputLists.Add(BossInputList[i]);
        }
        if(currTurnNum < BossActionList.Count-1)inputLists.Add(BossData.BossActions[BossActionList[currTurnNum+1]].BossActions[0]);
    }

    private IEnumerator HandleBossInput(int value)
    {
        if(IsPlayerDie) yield break;
        if (value == -1) yield break;
        switch (value)
        {
            case BossInputType.ATTACK10:
                Boss.DoAttack1Pre();
                break;
            case BossInputType.ATTACK11:
                Boss.DoAttack1Step(1);
                break;
            case BossInputType.ATTACK12:
                Boss.DoAttack1Step(2);
                break;
            case BossInputType.ATTACK13:
                Boss.DoAttack1Step(3);
                break;
            case BossInputType.ATTACK14:
                Boss.DoAttack1Step(4);
                break;
            case BossInputType.ATTACK15:
                Boss.DoAttack1Step(5);
                break;
            case BossInputType.ATTACK20:
                Boss.DoAttack2Pre();
                break;
            case BossInputType.ATTACK21:
                Boss.DoAttack2Step(1);
                break;
            case BossInputType.ATTACK22:
                Boss.DoAttack2Step(2);
                break;
            case BossInputType.ATTACK23:
                Boss.DoAttack2Step(3);
                break;
            case BossInputType.ATTACK24:
                Boss.DoAttack2Step(4);
                break;
            case BossInputType.ATTACK25:
                Boss.DoAttack2Step(5);
                break;
            case BossInputType.ATTACK30:
                yield return StartCoroutine(WaitForTask(Boss.DoAttack3Pre()));
                break;
            case BossInputType.ATTACK31:
                yield return StartCoroutine(WaitForTask(Boss.DoAttack3Step1(true)));
                break;
            case BossInputType.ATTACK32:
                yield return StartCoroutine(WaitForTask(Boss.DoAttack3Step1(false)));
                break;
            case BossInputType.ATTACK33:
                Boss.DoAttack3Step(3);
                break;
            case BossInputType.ATTACK34:
                Boss.DoAttack3Step(4);
                break;
            case BossInputType.ATTACK35:
                Boss.DoAttack3Step(5);
                break;
            case BossInputType.ATTACK40:
                yield return StartCoroutine(WaitForTask(Boss.DoAttack4Pre()));
                break;
            case BossInputType.ATTACK41:
                yield return StartCoroutine(WaitForTask(Boss.DoAttack4Step1(true)));
                break;
            case BossInputType.ATTACK42:
                Boss.DoAttack4Step(2);
                yield return StartCoroutine(WaitForTask(Boss.DoAttack4Step1(false)));
                break;
            case BossInputType.ATTACK43:
                Boss.DoAttack4Step(3);
                break;
            case BossInputType.ATTACK44:
                Boss.DoAttack4Step(4);
                break;
            case BossInputType.ATTACK45:
                Boss.DoAttack4Step(5);
                break;
            case BossInputType.ATTACK50:
                Boss.DoAttack5Pre();
                break;
            
            
            ////
            case BossInputType.ATTACK51:
                Boss.DoAttack5Step1();
                break;
            case BossInputType.ATTACK52:
                Boss.DoAttack5Step1();
                break;
            case BossInputType.ATTACK53:
                Boss.DoAttack5Step1();
                break;
            case BossInputType.ATTACK54:
                Boss.DoAttack5Step1();
                break;
            case BossInputType.ATTACK55:
                Boss.DoAttack5Step2();
                break;
            
            default:
                Debug.LogError("wrong boss attack type");
                break;
        }
    }
    
    private IEnumerator WaitForTask(Task task)
    {
        if(task==null) yield break;
        while (!task.IsCompleted && !IsPlayerDie)
        {
            yield return null; // 等待任务完成
        }
    }
    

    private Task HandlePlayerInput(int value)
    {
        if (value == -1 || IsPlayerDie) return null;
        // todo 跳跃攻击
            switch (value)
            {
                case PlayerInputType.MOVEA: 
                    return Player.DoMove(new int2(-1,0));
                case PlayerInputType.MOVED: 
                    return Player.DoMove(new int2(1,0));
                case PlayerInputType.ATTACK1: 
                    Player.DoHorizontalAttack();
                    return null;
                case PlayerInputType.ATTACK2: 
                    Player.DoCrossAttack();
                    return null;
                case PlayerInputType.DEFENSE:
                    Player.DoProtected();
                    return null;
                case PlayerInputType.HEAL:
                    Player.DoHeal();
                    return null;
                case PlayerInputType.JUMP:
                    Player.DoJump();
                    return null;
                case PlayerInputType.JUMPATTACK:
                    Player.DoJump();
                    return null;
                default:
                    Debug.LogError("wrong player attack type");
                    return null;
            }
    }

    /// <summary>
    /// 对战过程
    /// </summary>
    /// <returns></returns>
    IEnumerator BattleCoroutine()
    {
        if (IsPlayerDie) yield break;
        if (currTurnNum == 0)
        {
            BossStartPoss();
            yield return StartCoroutine(HandleBossInput(inputLists[0]));
        }
        else
        {
            ReorderInput();
            
            for (int i = 0; i < inputLists.Count; i++)
            {
                if (i % 2 == 0)
                {
                    // PlayerInput.HighlightInputButton(i/2);
                    yield return StartCoroutine(WaitForTask(HandlePlayerInput(inputLists[i])));
                    // yield return HandlePlayerInput(inputLists[i]);
                    Player.HandleLastJump();
                }
                else
                {
                    // if(i/2-1>=0)PlayerInput.SetBackInputButton(i / 2 - 1);
                    yield return StartCoroutine(HandleBossInput(inputLists[i]));
                }   
                yield return new WaitForSecondsRealtime(1f);
            }   
        }
        
        PlayerInput.ClearMemoryList();
        
        BossInputList.Clear();
        PlayerInputList.Clear();
        inputLists.Clear();
        
        Player.ResetPlayerState();
        currTurnNum++;
        if (currTurnNum > 19) currTurnNum -= 10;

        CurrTurnLabel.text = currTurnNum.ToString();
        // 重新开始下一轮
        HandlePlayerTurn();
    }

    public void Restart()
    {
        BossAnimator.Play("bossIdle");
        // BossAnimator.CrossFade("bossIdle",0.1f);
        
        PlayerInput.ClearMemoryList();
        BossInputList.Clear();
        PlayerInputList.Clear();
        inputLists.Clear();
        Player.Reset();

        PlayerHp_UI.Clear();
        
        BossHp_UI.Setup(bossStartHp);
        PlayerHp_UI.Setup(playerStartHp);
        currTurnNum = 0;
        
        HandlePlayerTurn();
        StartCoroutine(BattleCoroutine());

        HandlePlayerDie(false);
    }

    public void HandlePlayerDie(bool value)
    {
        PlayerDeathPanel.SetActive(value);
        GamePanel.SetActive(!value);
        IsPlayerDie = value;
    }
    
}
