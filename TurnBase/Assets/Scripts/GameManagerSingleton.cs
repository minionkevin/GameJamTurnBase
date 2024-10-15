using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections;

public class GameManagerSingleton : BaseSingleton<GameManagerSingleton>
{
    // 这里的大部分数据应该都是private+从data里read
    // 写成public只是为了测试方便
    public int Width = 7;
    public int Height = 6;

    public int2 bossHeadPos;
    public int2 bossLeftHandPos;
    public int2 bossRightHandPos;

    public int bossStartHp = 40;
    public int playerStartHp = 4;       // 玩家初始血量（心形的个数）
    public int countdownTime = 40;      
    
    // 有了config之后从scriptableobject read
    public int2 PlayerStartPos;
    
    public GameObject PlayerPrefab;
    public GameObject BossPrefab;

    public GameObject BossHeadPrefab;
    public GameObject BossLeftHandPrefab;
    public GameObject BossRightHandPrefab;

    public CountDown CountDown_UI;
    public BossHp BossHp_UI;
    public PlayerHp PlayerHp_UI;

    public PlayerComponent Player;
    public BossComponent Boss;
    public List<int> PlayerInputList = new List<int>();
    public List<int> BossInputList = new List<int>();

    private List<int> inputLists = new List<int>();
    
    public PlayerInputComponent PlayerInput;
    
    

    void Start()
    {
        // tile spawn
        TileManagerSingleton.Instance.Setup(Width, Height);
        
        // player spawn
        var player = Instantiate(PlayerPrefab);
        var playerComponent = player.GetComponent<PlayerComponent>();
        Player = playerComponent;
        playerComponent.Setup(PlayerStartPos);
        TileManagerSingleton.Instance.AddObjectToTile(PlayerStartPos,player);

        // boss spawn
        // 这里的boss只是一个数据载体，剩余的三个boss prefab才是视觉上看到的
        var boss = Instantiate(BossPrefab);
        boss.GetComponent<Transform>().SetParent(transform);
        var bossComponent = boss.GetComponent<BossComponent>();
        Boss = bossComponent;

        var bossHead = Instantiate(BossHeadPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossHeadPos,bossHead);
        var bossLeftHand = Instantiate(BossLeftHandPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossLeftHandPos,bossLeftHand);
        var bossRightHand = Instantiate(BossRightHandPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossRightHandPos,bossRightHand);
        bossComponent.Setup(bossHeadPos,bossLeftHandPos,bossRightHandPos,bossHead,bossLeftHand,bossRightHand);

        // player hp & boss hp spawn
        BossHp_UI.Setup(bossStartHp);
        PlayerHp_UI.Setup(playerStartHp);

        
        EnterGame();
    }

    /// <summary>
    /// 开启游戏计时，启用玩家输入，正式进入游戏
    /// </summary>
    public void EnterGame()
    {
        // Boss 摆出Pose

        // 开启游戏计时
        CountDown_UI.Setup(countdownTime);
        // 启用玩家输入
        PlayerInputComponent.InputEnabled = true;
    }

    /// <summary>
    /// 进入对战（计时结束或者直接确认）
    /// </summary>
    public void EnterBattle()
    {
        // 禁用一切输入
        PlayerInputComponent.InputEnabled = false;
        // 计时器暂停（等到下一回合重置）
        CountDown.Instance.SetTimerPause();

        // 进入对战
        StartCoroutine(BattleCoroutine());
    }

    /// <summary>
    /// 交替重排指令
    /// </summary>
    private void ReorderInput()
    {
        // todo read boss指令data
        // BossInputList.Add(BossInputType.ATTACK1);
        
        BossInputList.Add(BossInputType.ATTACK10);
        BossInputList.Add(BossInputType.ATTACK11);
        BossInputList.Add(BossInputType.ATTACK12);
        BossInputList.Add(BossInputType.ATTACK13);
        BossInputList.Add(BossInputType.ATTACK14);
        BossInputList.Add(BossInputType.ATTACK15);
        
        for (int i = 0; i < Math.Max(BossInputList.Count, PlayerInputList.Count) * 2; i++)
        {
            if(i < PlayerInputList.Count) inputLists.Add(PlayerInputList[i]);
            if(i < BossInputList.Count) inputLists.Add(BossInputList[i]);
        }
    }

    private void HandleBossInput(int value)
    {
        switch (value)
        {
            case BossInputType.ATTACK10:
                Boss.DoAttack1Pre();
                break;
            case BossInputType.ATTACK11:
                Boss.DoAttack1Step1();
                break;
            case BossInputType.ATTACK12:
                Boss.HandleVerticalAttackAndMove(true);
                break;
            case BossInputType.ATTACK13:
                Boss.HandleVerticalAttackAndMove(false);
                break;
            case BossInputType.ATTACK14:
                Boss.HandleVerticalMove(true);
                break;
            case BossInputType.ATTACK15:
                Boss.HandleVerticalMove(false);
                break;
            default:
                Debug.LogError("wrong boss attack type");
                break;
        }
    }

    private void HandlePlayerInput(int value)
    {
        // todo 跳跃攻击
            switch (value)
            {
                case PlayerInputType.MOVEA: 
                    Player.DoMove(new int2(-1,0));
                    break;
                case PlayerInputType.MOVED: 
                    Player.DoMove(new int2(1,0));
                    break;
                case PlayerInputType.ATTACK1: 
                    Player.DoHorizontalAttack();
                    break;
                case PlayerInputType.ATTACK2: 
                    Player.DoCrossAttack();
                    break;
                case PlayerInputType.DEFENSE:
                    Player.DoProtected();
                    break;
                case PlayerInputType.HEAL:
                    Player.DoHeal();
                    break;
                case PlayerInputType.JUMP:
                    Player.DoJump();
                    break;
                case PlayerInputType.JUMPATTACK:
                    Player.DoJump();
                    break;
                default:
                    Debug.LogError("wrong player attack type");
                    break;
            }
    }

    /// <summary>
    /// 对战过程
    /// </summary>
    /// <returns></returns>
    IEnumerator BattleCoroutine()
    {
        // 每做完一个指令，等待所有动画完成，再开始下一个指令

        ReorderInput();
        
        // todo safety check
        for (int i = 0; i < inputLists.Count; i++)
        {
            if(i%2 == 0)HandlePlayerInput(inputLists[i]);
            else HandleBossInput(inputLists[i]);
        
            yield return new WaitForSecondsRealtime(3);
        }

        // boss测试
        // for (int i = 0; i < BossInputList.Count; i++)
        // {
        //     HandleBossInput(BossInputList[i]);   
        //     yield return new WaitForSecondsRealtime(3);
        // }
        
        
        yield return new WaitForFixedUpdate();
        
        PlayerInput.ClearMemoryList();
        
        yield return null;

        // 重新开始下一轮
        EnterGame();
    }


}
