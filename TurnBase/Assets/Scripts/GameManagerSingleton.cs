using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;

public class GameManagerSingleton : BaseSingleton<GameManagerSingleton>
{
    // 这里的大部分数据应该都是private+从data里read
    // 写成public只是为了测试方便
    public int Width = 7;
    public int Height = 6;

    public List<int> BossActionList = new List<int>();
    public BossActionScriptableObject BossData;

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

    private int currTurnNum;

    void Start()
    {
        currTurnNum = 0;
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

    
    // todo timer check
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

        HandleBossActions();
        // 进入对战
        StartCoroutine(BattleCoroutine());
    }

    private void HandleBossActions()
    {
        foreach (var data in BossData.BossActions[BossActionList[currTurnNum]].BossActions)
        {
            BossInputList.Add(data);
        }
    }

    /// <summary>
    /// 交替重排指令
    /// </summary>
    private void ReorderInput()
    {
        for (int i = 0; i < Math.Max(BossInputList.Count, PlayerInputList.Count) * 2; i++)
        {
            if(i < BossInputList.Count) inputLists.Add(BossInputList[i]);
            if(i < PlayerInputList.Count) inputLists.Add(PlayerInputList[i]);
        }
    }

    private async void HandleBossInput(int value)
    {
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
                await Boss.DoAttack3Pre();
                break;
            case BossInputType.ATTACK31:
                Boss.DoAttack3Step(1);
                break;
            case BossInputType.ATTACK32:
                Boss.DoAttack3Step(2);
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
                await Boss.DoAttack4Pre();
                break;
            case BossInputType.ATTACK41:
                Boss.DoAttack4Step(1);
                break;
            case BossInputType.ATTACK42:
                Boss.DoAttack4Step(2);
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
            
            default:
                Debug.LogError("wrong boss attack type");
                break;
        }
    }

    private Task HandlePlayerInput(int value)
    {
        // todo 跳跃攻击
            switch (value)
            {
                case PlayerInputType.MOVEA: 
                    return Player.DoMove(new int2(-1,0));
                case PlayerInputType.MOVED: 
                    return Player.DoMove(new int2(1,0));
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
            return null;
    }

    /// <summary>
    /// 对战过程
    /// </summary>
    /// <returns></returns>
    IEnumerator BattleCoroutine()
    {
        // 每做完一个指令，等待所有动画完成，再开始下一个指令

        ReorderInput();
        //
        // // todo safety check
        // switch back to i%2==0 for player after boss pose finish
        for (int i = 0; i < inputLists.Count; i++)
        {
            if (i % 2 != 0) yield return HandlePlayerInput(inputLists[i]);
            else HandleBossInput(inputLists[i]);
        
            yield return new WaitForSecondsRealtime(1f);
        }
        
        PlayerInput.ClearMemoryList();
        BossInputList.Clear();
        PlayerInputList.Clear();
        
        yield return null;
        
        currTurnNum++;
        // 重新开始下一轮
        EnterGame();
    }


}
