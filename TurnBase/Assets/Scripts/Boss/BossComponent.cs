using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BossComponent : MonoBehaviour
{
    private int2 currHeadPos;
    private int2 currLeftHandPos;
    private int2 currRightHandPos;

    private GameObject headObj;
    private GameObject leftHandObj;
    private GameObject rightHandObj;

    // 需要reset
    private bool isPlayerOnLeft;
    private int attack1yPos;

    private int width;
    private int height;

    public void Setup(int2 bossHeadPos, int2 bossLeftHandPos, int2 bossRightHandPos,GameObject head, GameObject leftHand, GameObject rightHand)
    {
        currHeadPos = bossHeadPos;
        currLeftHandPos = bossLeftHandPos;
        currRightHandPos = bossRightHandPos;
        headObj = head;
        leftHandObj = leftHand;
        rightHandObj = rightHand;
        width = GameManagerSingleton.Instance.Width;
        height = GameManagerSingleton.Instance.Height;
    }

    #region attack1-连续下劈
    public void DoAttack1Step(int step)
    {
        switch (step)
        {
            case 1:
                DoAttackSingleHand();
                break;
            case 2:
                DoAttackAndMoveHand(Attack1StepCheck(true));
                break;
            case 3:
                DoAttackAndMoveHand(Attack1StepCheck(false));
                break;
            case 4:
                HandleVerticalMove(true);
                break;
            case 5:
                HandleVerticalMove(false);
                break;
            
        }
    }
    
    public void DoAttack1Pre()
    {
        MoveHandToPlayerTop();
    }

    private async void DoAttackSingleHand()
    {
        if(isPlayerOnLeft) await DoVerticalAttack(leftHandObj, currLeftHandPos,true);
        else await DoVerticalAttack(rightHandObj,currRightHandPos,false);
    }
    
    private bool Attack1StepCheck(bool isFirstTime)
    {
        bool tmp = false;
        switch (isPlayerOnLeft)
        {
            case true when isFirstTime:
            case false when !isFirstTime:
                tmp = true;
                break;
        }
        return tmp;
    }

    private void HandleVerticalMove(bool isFirstTime)
    {
        switch (isFirstTime)
        {
            case true when isPlayerOnLeft:
                DoAttackAndResetHand(rightHandObj, ref currRightHandPos, leftHandObj, ref currLeftHandPos, false, false);
                break;
            case false when isPlayerOnLeft:
                DoAttackAndResetHand(leftHandObj, ref currLeftHandPos, rightHandObj, ref currRightHandPos, true, true);
                break;
            case true when !isPlayerOnLeft:
                DoAttackAndResetHand(leftHandObj, ref currLeftHandPos, rightHandObj, ref currRightHandPos, true, false);
                break;
            case false when !isPlayerOnLeft:
                DoAttackAndResetHand(rightHandObj, ref currRightHandPos, leftHandObj, ref currLeftHandPos, false, true);
                break;
        }
    }

    private async void DoAttackAndMoveHand(bool value)
    {
        if (value)
        {
            await DoVerticalAttack(rightHandObj,currRightHandPos,false);
            MoveObject(leftHandObj, isPlayerOnLeft ? currRightHandPos.x + 1 : currRightHandPos.x - 1, attack1yPos, ref currLeftHandPos);
        }
        else
        {
            await DoVerticalAttack(leftHandObj, currLeftHandPos,true);
            MoveObject(rightHandObj,isPlayerOnLeft?currLeftHandPos.x + 1:currLeftHandPos.x - 1,attack1yPos,ref currRightHandPos);
        }
    }

    private void DoAttackAndResetHand(GameObject attackHand, ref int2 attackPos, GameObject moveHand, ref int2 movePos, bool isAttackHandLeft, bool value)
    {
        if (value) DoVerticalAttack(attackHand, attackPos, isAttackHandLeft);
        else
        {
            DoVerticalAttack(attackHand, attackPos, isAttackHandLeft);
            MoveObject(moveHand, movePos.x, attack1yPos, ref movePos);
        }
    }
    #endregion
    
    #region attack2-左右双掌 
    public void DoAttack2Pre()
    {
        MoveObject(leftHandObj, 1, height - 2, ref currLeftHandPos);
        MoveObject(rightHandObj,width-2,height-2,ref currRightHandPos);
        MoveObject(headObj,width/2,height-1,ref currHeadPos);
    }
    public async void DoAttack2Step(int step)
    {
        switch (step)
        {
            case 1:
                await DoAOEAttackBack(leftHandObj, currLeftHandPos, true,false);
                break;
            case 2:
                await DoAOEAttackBack(rightHandObj, currRightHandPos, false,false);
                MoveObject(leftHandObj, 2, height - 2, ref currLeftHandPos);
                break;
            case 3:
                await DoAOEAttackBack(leftHandObj, currLeftHandPos, true,false);
                MoveObject(rightHandObj, width - 3, height - 2, ref currRightHandPos);
                break;
            case 4:
                await DoAOEAttackBack(rightHandObj, currRightHandPos, false,false);
                MoveObject(leftHandObj, 1, height - 2, ref currLeftHandPos);
                break;
            case 5:
                MoveObject(rightHandObj, width - 2, height - 2, ref currRightHandPos);
                await DoHeadVerticalAttack(); 
                await MoveObject(headObj, currHeadPos.x, 1, ref currHeadPos);
                break;
        }
    }
    #endregion

    #region attack3-全屏激光
    public void DoAttack3Step(int step)
    {
        switch (step)
        {
            case 1:
                DoAttack3Step1();
                break;
            case 2:
                DoAttack3Step1();
                break;
            case 3:
                DoAttack3MoveHand();
                break;
            case 4:
            case 5:
                DoAttack3HandAttack();
                break;
            
        }
    }
    
    public void DoAttack3Pre()
    {
        SetupBossStartPos(new int2(1, height - 2),
            new int2(width-2,height-2),
            new int2(width/2, 1));
    }

    private void DoAttack3Step1()
    {
        // 激光
        // 第一步和第二步只是视觉上的区别，数据上都是一样的
        List<int2> attackList = new List<int2>();
        int targetPos = TileManagerSingleton.Instance.GetIndexPos(new int2(width / 2, 0));
        foreach (var tile in TileManagerSingleton.Instance.TileList)
        {
            int index = tile.GetTileIndex();
            if(index != targetPos) attackList.Add(index);
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
    }

    private async void DoAttack3MoveHand()
    {
        await Task.WhenAll(MoveObject(rightHandObj, currRightHandPos.x - 1, currHeadPos.y + 1, ref currRightHandPos),
            MoveObject(leftHandObj, currLeftHandPos.x + 1, currHeadPos.y + 1, ref currLeftHandPos));
    }

    private async void DoAttack3HandAttack()
    {
        await Task.WhenAll(DoAOEAttackBack(leftHandObj, currLeftHandPos, true), DoAOEAttackBack(rightHandObj, currRightHandPos, false));
    }
    #endregion
    
    # region attack4-地面清扫
    public void DoAttack4Pre()
    {
        SetupBossStartPos(new int2(1, height - 2),
            new int2(width-2,height-2),
               new int2(width/2, height - 2));
    }

    public async void DoAttack4Step(int step)
    {
        switch (step)
        {
            case 1:
                DoAttack4Step1();
                break;
            case 2:
                await MoveObject(headObj,currHeadPos.x,currHeadPos.y-1,ref currHeadPos);
                DoAttack4Step1();
                break;
            case 3:
                await MoveObject(leftHandObj, currLeftHandPos.x, 0, ref currLeftHandPos);
                await DoHorizontalAttack(leftHandObj, currLeftHandPos, true);
                MoveObject(leftHandObj, width - 1, height - 1, ref currLeftHandPos);
                break;
            case 4:
                await MoveObject(rightHandObj, currRightHandPos.x, 0, ref currRightHandPos);
                await DoHorizontalAttack(rightHandObj, currRightHandPos, false);
                break;
            case 5:
                DoAttack4Step3();
                break;
        }
    }
    
    private void DoAttack4Step1()
    {
        // 激光
        List<int2> attackList = new List<int2>();
        int targetPos1 = TileManagerSingleton.Instance.GetIndexPos(new int2(width / 2, 0));
        int targetPos2 = TileManagerSingleton.Instance.GetIndexPos(new int2(width / 2, 1));
        int targetPos3 = TileManagerSingleton.Instance.GetIndexPos(new int2(width / 2-1, 0));
        int targetPos4 = TileManagerSingleton.Instance.GetIndexPos(new int2(width / 2+1, 0));
        
        foreach (var tile in TileManagerSingleton.Instance.TileList)
        {
            int index = tile.GetTileIndex();
            if(index != targetPos1 || index != targetPos2||index!=targetPos3||index!=targetPos4) attackList.Add(index);
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
        
        // 播放动画
        // 播放音效
    }
    
    private void DoAttack4Step3()
    {
        MoveObject(headObj, width/2, height - 1, ref currHeadPos);
        MoveObject(leftHandObj, currHeadPos.x-2, height - 2, ref currLeftHandPos);
        MoveObject(rightHandObj,currHeadPos.x+2,height-2,ref currRightHandPos);
    }
    #endregion

    #region attack5-范围拍击
    public void DoAttack5Pre()
    {
        MoveObject(headObj, width / 2, 0, ref currHeadPos);
        MoveHandToPlayerTop();
    }

    public async void DoAttack5Step1()
    {
        if (isPlayerOnLeft)
        {
            int2 newPos = new int2(currRightHandPos.x + 1, currRightHandPos.y);
            if (!CheckLimit(newPos)) return;
            if (!newPos.Equals(currRightHandPos)) await MoveObject(rightHandObj, newPos.x, newPos.y, ref currRightHandPos);
        }
        else
        {
            int2 newPos = new int2(currLeftHandPos.x - 1, currLeftHandPos.y);
            if (!CheckLimit(newPos)) return;
            if (!newPos.Equals(currLeftHandPos)) await MoveObject(leftHandObj, newPos.x, newPos.y, ref currLeftHandPos);
        }
        await Task.WhenAll(DoAOEAttackBack(leftHandObj, currLeftHandPos, true), DoAOEAttackBack(rightHandObj, currRightHandPos, false));
    }

    public async void DoAttack5Step2()
    {
        MoveObject(rightHandObj, width - 2, height - 2, ref currRightHandPos);
        
        await DoHeadVerticalAttack(); 
        await MoveObject(headObj, currHeadPos.x, 1, ref currHeadPos);
    }

    #endregion

    /// <summary>
    /// 轮次实际动作做完后，在队尾默认添加的一个指令
    /// </summary>
    public void DoActionEnd()
    {
        // Do Nothing now.
        // 先让Boss手回归原位吧！
        MoveObject(leftHandObj, GameManagerSingleton.Instance.bossLeftHandPos.x, GameManagerSingleton.Instance.bossLeftHandPos.y, ref currLeftHandPos);
        MoveObject(rightHandObj, GameManagerSingleton.Instance.bossRightHandPos.x, GameManagerSingleton.Instance.bossRightHandPos.y, ref currRightHandPos);
    }
    
    
    private void SetupBossStartPos(int2 leftHandPos,int2 rightHandPos, int2 headPos)
    {
        MoveObject(headObj, headPos, ref currHeadPos);
        MoveObject(leftHandObj, leftHandPos, ref currLeftHandPos);
        MoveObject(rightHandObj, rightHandPos, ref currRightHandPos);
    }
    
    // dynamic移动手到玩家上方
    private void MoveHandToPlayerTop()
    {
        int2 playerPos = GameManagerSingleton.Instance.Player.GetPlayerPos(); 
        
        if (playerPos.x < width / 2)
        {
            MoveObject(leftHandObj, playerPos.x, height-2,ref currLeftHandPos);
            MoveObject(rightHandObj,playerPos.x+1,height-2,ref currRightHandPos);
            
            isPlayerOnLeft = true;
            attack1yPos = currLeftHandPos.y;
        }
        else
        {
            MoveObject(rightHandObj,playerPos.x, height-2,ref currRightHandPos);
            MoveObject(leftHandObj, currRightHandPos.x-1, height-2,ref currLeftHandPos);
            
            attack1yPos = currRightHandPos.y;
        }
    }
    
    // 移动物体位置
    private Task MoveObject(GameObject handObj, int x, int y, ref int2 currentHandPos)
    {
        if (!CheckLimit(x, y)) return null;
        int2 newPos = new int2(x, y);
        currentHandPos = newPos;
        return TileManagerSingleton.Instance.MoveObjectToTile(new int2(x,y),handObj);
    }
    private Task MoveObject(GameObject handObj, int2 pos, ref int2 currentHandPos)
    {
        if (!CheckLimit(pos)) return null;
        currentHandPos = pos;
        return TileManagerSingleton.Instance.MoveObjectToTile(pos,handObj);
    }
    
    // 头部垂直攻击
    private Task DoHeadVerticalAttack()
    {
        List<int2> attackList = new List<int2>();
        while (currHeadPos.y >= 0)
        {
            int2 targetPos = new int2(currHeadPos.x, currHeadPos.y--);
            if (CheckLimit(targetPos)) attackList.Add(targetPos);
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
        currHeadPos = attackList[^1];
        return TriggerVerticalAttackAnimation(attackList[^1],headObj);
    }

    // 手部垂直攻击
    private Task DoVerticalAttack(GameObject obj, int2 pos,bool value)
    {
        List<int2> attackList = new List<int2>();
        for (int i = 0; i < 3; i++)
        {
            int2 targetPos = new int2(pos.x, pos.y - i);
            if (CheckLimit(targetPos))
            {
                attackList.Add(targetPos);
            }
            else break;
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList, 2);
        
        if (value) currLeftHandPos = attackList[^1];
        else currRightHandPos = attackList[^1];
        
        return TriggerVerticalAttackAnimation(attackList[^1],obj);
    }

    // 手部水平攻击
    private Task DoHorizontalAttack(GameObject obj, int2 pos,bool isLeft)
    {
        List<int2> attackList = new List<int2>();
        int direction = isLeft ? 1 : -1;
        for (int i = pos.x; i >= 0 && i < width; i += direction)
        {
            int2 targetPos = new int2(i, pos.y);
            if (CheckLimit(targetPos)) attackList.Add(targetPos);
        }
        
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
        
        if (isLeft) currLeftHandPos = attackList[^1];
        else currRightHandPos = attackList[^1];
        
        return TriggerVerticalAttackAnimation(attackList[^1],obj);
    }

    // 手部拍击
    private async Task DoAOEAttackBack(GameObject obj, int2 pos, bool isLeft,bool isBack = true)
    {
        int2 tmp = new int2();
        List<int2> attackList = new List<int2>();
        for (int i = 0; i < 3; i++)
        {
            int2 targetPos = new int2(pos.x, pos.y - i);
            if (CheckLimit(targetPos)) attackList.Add(targetPos);
        }
        
        tmp = attackList[^1];
        
        int2 sidePos = new int2(tmp.x - 1, tmp.y);
        if(CheckLimit(sidePos)) attackList.Add(sidePos);
        sidePos = new int2(tmp.x + 1 , tmp.y);
        if(CheckLimit(sidePos)) attackList.Add(sidePos);
        
        if (isLeft) currLeftHandPos = tmp;
        else currRightHandPos = tmp;

        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
        await TileManagerSingleton.Instance.AddObjectToTile(tmp,obj);

        if (isBack)
        {
            if (isLeft) currLeftHandPos = pos;
            else currRightHandPos = pos;
            await TileManagerSingleton.Instance.AddObjectToTile(pos,obj);   
        }
    }
    
    // placeholder animation
    private Task TriggerVerticalAttackAnimation(int2 targetPos, GameObject obj)
    {
        var TileManager = TileManagerSingleton.Instance;
        TileManager.TileList[TileManager.GetIndexPos(targetPos)].SetObjectToTile(obj);
        Transform currTrans = obj.GetComponent<Transform>();

        Sequence timeline = DOTween.Sequence();
        timeline.Insert(0, currTrans.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.Linear));
        return timeline.Play().AsyncWaitForCompletion();
    }
    
    private bool CheckLimit(int2 targetPos)
    {
        return targetPos.x >= 0 && targetPos.x < width && targetPos.y >= 0 && targetPos.y < height;
    }
    private bool CheckLimit(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    public void CheckForDamage(int value, List<int2> attackList)
    {
        foreach (int2 pos in attackList)
        {
            if(TileManagerSingleton.Instance.CheckPos(currHeadPos, pos))
            {
                GameManagerSingleton.Instance.BossHp_UI.OnTakeDamage(value*2);
            }
            else if (TileManagerSingleton.Instance.CheckPos(currLeftHandPos, pos) ||
                     TileManagerSingleton.Instance.CheckPos(currRightHandPos, pos))
            {
                GameManagerSingleton.Instance.BossHp_UI.OnTakeDamage(value);
            }
        }
    }
}

// 所有攻击都是，先攻击再移动，反正肯定不是同时发生