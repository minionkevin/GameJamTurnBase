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

    public Action PlayerActionList;
    

    public Action OnBattleStart;
    public Action OnBattleEnd;

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
        bossComponent.Setup(bossHeadPos,bossLeftHandPos,bossRightHandPos);

        var bossHead = Instantiate(BossHeadPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossHeadPos,bossHead);
        var bossLeftHand = Instantiate(BossLeftHandPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossLeftHandPos,bossLeftHand);
        var bossRightHand = Instantiate(BossRightHandPrefab);
        TileManagerSingleton.Instance.AddObjectToTile(bossRightHandPos,bossRightHand);

        // player hp & boss hp spawn
        BossHp_UI.Setup(bossStartHp);
        PlayerHp_UI.Setup(playerStartHp);

        OnBattleEnd += EnterGame;
        //
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

        // 根据Player的指令队列和Boss当前要使用的指令队列，交替重排行动


        // 进入对战
        StartCoroutine(BattleCoroutine());
    }

    /// <summary>
    /// 交替重排指令
    /// </summary>
    private void ReOrder_PlayerBossAction()
    {
        
    }

    /// <summary>
    /// 对战过程
    /// </summary>
    /// <returns></returns>
    IEnumerator BattleCoroutine()
    {
        OnBattleStart?.Invoke();

        PlayerActionList?.Invoke();
        PlayerActionList = null;

        // confirm之后需要block玩家输入
        // 直到下一轮到玩家
        yield return new WaitForFixedUpdate();
        // 这里应该有个callback
        // 当这件事完成之后，销毁这个，再去完成下一个，然后再销毁
        //foreach (var item in memoryList)
        //{
        //    Destroy(item);
        //}

        OnBattleEnd?.Invoke();
        yield return null;
    }


}
