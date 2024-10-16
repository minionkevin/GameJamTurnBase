using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

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

    public void Setup(int2 bossHeadPos, int2 bossLeftHandPos, int2 bossRightHandPos,GameObject head, GameObject leftHand, GameObject rightHand)
    {
        currHeadPos = bossHeadPos;
        currLeftHandPos = bossLeftHandPos;
        currRightHandPos = bossRightHandPos;
        headObj = head;
        leftHandObj = leftHand;
        rightHandObj = rightHand;
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

    private void DoAttackSingleHand()
    {
        if(isPlayerOnLeft) DoVerticalAttack(leftHandObj, currLeftHandPos,true);
        else DoVerticalAttack(rightHandObj,currRightHandPos,false);
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

    private void DoAttackAndMoveHand(bool value)
    {
        if (value)
        {
            DoVerticalAttack(rightHandObj,currRightHandPos,false);
            MoveHand(leftHandObj, isPlayerOnLeft ? currRightHandPos.x + 1 : currRightHandPos.x - 1, attack1yPos, ref currLeftHandPos);
        }
        else
        {
            DoVerticalAttack(leftHandObj, currLeftHandPos,true);
            MoveHand(rightHandObj,isPlayerOnLeft?currLeftHandPos.x + 1:currLeftHandPos.x - 1,attack1yPos,ref currRightHandPos);
        }
    }

    private void DoAttackAndResetHand(GameObject attackHand, ref int2 attackPos, GameObject moveHand, ref int2 movePos, bool isAttackHandLeft, bool value)
    {
        if (value) DoVerticalAttack(attackHand, attackPos, isAttackHandLeft);
        else
        {
            // 移动另一只手并攻击
            MoveHand(moveHand, movePos.x, attack1yPos, ref movePos);
            DoVerticalAttack(attackHand, attackPos, isAttackHandLeft);
        }
    }
    #endregion
    
    #region attack2-左右双掌 
    public void DoAttack2Pre()
    {
        MoveHand(leftHandObj, 1, GameManagerSingleton.Instance.Height - 2, ref currLeftHandPos);
        MoveHand(rightHandObj,GameManagerSingleton.Instance.Width-2,GameManagerSingleton.Instance.Height-2,ref currRightHandPos);
        MoveHand(headObj,GameManagerSingleton.Instance.Width/2,GameManagerSingleton.Instance.Height-1,ref currHeadPos);
    }
    public void DoAttack2Step(int step)
    {
        switch (step)
        {
            case 1:
                DoAOEAttack(leftHandObj, currLeftHandPos, true);
                break;
            case 2:
                DoAOEAttack(rightHandObj, currRightHandPos, false);
                MoveHand(leftHandObj, 2, GameManagerSingleton.Instance.Height - 2, ref currLeftHandPos);
                break;
            case 3:
                DoAOEAttack(leftHandObj, currLeftHandPos, true);
                MoveHand(rightHandObj, GameManagerSingleton.Instance.Width - 3, GameManagerSingleton.Instance.Height - 2, ref currRightHandPos);
                break;
            case 4:
                DoAOEAttack(rightHandObj, currRightHandPos, false);
                MoveHand(leftHandObj, 1, GameManagerSingleton.Instance.Height - 2, ref currLeftHandPos);
                break;
            case 5:
                MoveHand(rightHandObj, GameManagerSingleton.Instance.Width - 2, GameManagerSingleton.Instance.Height - 2, ref currRightHandPos);
                DoHeadVerticalAttack(); 
                MoveHand(headObj, currHeadPos.x, 1, ref currHeadPos);
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
    
    public async Task DoAttack3Pre()
    {
        // 检查玩家血量
        // todo 放置真的前摇动作
        MoveHand(headObj, GameManagerSingleton.Instance.Width/2, 1, ref currHeadPos);
        MoveHand(leftHandObj, 1, GameManagerSingleton.Instance.Height - 2, ref currLeftHandPos);
        MoveHand(rightHandObj,GameManagerSingleton.Instance.Width-2,GameManagerSingleton.Instance.Height-2,ref currRightHandPos);
        
        var bossHeadSprite = headObj.GetComponent<SpriteRenderer>();
        await bossHeadSprite.DOColor(Color.white, 0.1f).SetLoops(3).AsyncWaitForCompletion();
        bossHeadSprite.color = Color.red;
    }

    private void DoAttack3Step1()
    {
        // 激光
        // 第一步和第二步只是视觉上的区别，数据上都是一样的
        List<int2> attackList = new List<int2>();
        int targetPos = TileManagerSingleton.Instance.GetIndexPos(new int2(GameManagerSingleton.Instance.Width / 2, 0));
        foreach (var tile in TileManagerSingleton.Instance.TileList)
        {
            int index = tile.GetTileIndex();
            if(index != targetPos) attackList.Add(index);
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
    }

    private async void DoAttack3MoveHand()
    {
        await MoveHand(rightHandObj,currRightHandPos.x-1,currHeadPos.y+1,ref currRightHandPos);
        await MoveHand(leftHandObj,currLeftHandPos.x+1,currHeadPos.y+1,ref currLeftHandPos);
    }

    private void DoAttack3HandAttack()
    {
        DoAOEAttackBack(leftHandObj, currLeftHandPos, true);
        DoAOEAttackBack(rightHandObj, currRightHandPos, false);
    }
    #endregion
    
    # region attack4-地面清扫
    public async Task DoAttack4Pre()
    {
        // 检查玩家血量
        // todo 放置真的前摇动作
        MoveHand(headObj, GameManagerSingleton.Instance.Width/2, GameManagerSingleton.Instance.Height - 1, ref currHeadPos);
        MoveHand(leftHandObj, 1, GameManagerSingleton.Instance.Height - 1, ref currLeftHandPos);
        MoveHand(rightHandObj,GameManagerSingleton.Instance.Width-2,GameManagerSingleton.Instance.Height-1,ref currRightHandPos);
        
        var bossHeadSprite = headObj.GetComponent<SpriteRenderer>();
        await bossHeadSprite.DOColor(Color.blue, 0.1f).SetLoops(3).AsyncWaitForCompletion();
        bossHeadSprite.color = Color.red;
    }

    public void DoAttack4Step(int step)
    {
        switch (step)
        {
            case 1:
                DoAttack4Step1();
                break;
            case 2:
                MoveHand(headObj,currHeadPos.x,currHeadPos.y-1,ref currHeadPos);
                DoAttack4Step1();
                
                // hand pos?
                MoveHand(leftHandObj,currHeadPos.x-2,0,ref currLeftHandPos);
                MoveHand(rightHandObj,currHeadPos.x+2,0,ref currRightHandPos);
                break;
            case 3:
                DoHorizontalAttack(leftHandObj, currLeftHandPos, true);
                break;
            case 4:
                DoHorizontalAttack(rightHandObj, currRightHandPos, false);
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
        int targetPos1 = TileManagerSingleton.Instance.GetIndexPos(new int2(GameManagerSingleton.Instance.Width / 2, 0));
        int targetPos2 = TileManagerSingleton.Instance.GetIndexPos(new int2(GameManagerSingleton.Instance.Width / 2, 1));
        int targetPos3 = TileManagerSingleton.Instance.GetIndexPos(new int2(GameManagerSingleton.Instance.Width / 2-1, 0));
        int targetPos4 = TileManagerSingleton.Instance.GetIndexPos(new int2(GameManagerSingleton.Instance.Width / 2+1, 0));
        
        foreach (var tile in TileManagerSingleton.Instance.TileList)
        {
            int index = tile.GetTileIndex();
            if(index != targetPos1 || index != targetPos2||index!=targetPos3||index!=targetPos4) attackList.Add(index);
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
    }
    
    private void DoAttack4Step3()
    {
        MoveHand(headObj, GameManagerSingleton.Instance.Width/2, GameManagerSingleton.Instance.Height - 1, ref currHeadPos);
        MoveHand(leftHandObj, currHeadPos.x-2, GameManagerSingleton.Instance.Height - 2, ref currLeftHandPos);
        MoveHand(rightHandObj,currHeadPos.x+2,GameManagerSingleton.Instance.Height-2,ref currRightHandPos);
    }
    #endregion

    public void DoAttack5Pre()
    {
        MoveHandToPlayerTop();
    }

    public async void DoAttack5Step1()
    {
        if (isPlayerOnLeft)
        {
            int2 newPos = new int2(currRightHandPos.x + 1, currRightHandPos.y);
            if (!CheckLimit(newPos)) return;
            if (!newPos.Equals(currRightHandPos)) await MoveHand(rightHandObj, newPos.x, newPos.y, ref currRightHandPos);
        }
        else
        {
            int2 newPos = new int2(currLeftHandPos.x - 1, currLeftHandPos.y);
            if (!CheckLimit(newPos)) return;
            if (!newPos.Equals(currLeftHandPos)) await MoveHand(leftHandObj, newPos.x, newPos.y, ref currLeftHandPos);
        }
        
        DoAOEAttackBack(leftHandObj, currLeftHandPos, true);
        DoAOEAttackBack(rightHandObj, currRightHandPos, false);
    }

    public void DoAttack5Step2()
    {
        MoveHand(rightHandObj, GameManagerSingleton.Instance.Width - 2, GameManagerSingleton.Instance.Height - 2, ref currRightHandPos);
        DoHeadVerticalAttack(); 
        MoveHand(headObj, currHeadPos.x, 1, ref currHeadPos);
    }

    private void MoveHandToPlayerTop()
    {
        int2 playerPos = GameManagerSingleton.Instance.Player.GetPlayerPos(); 
        
        if (playerPos.x < GameManagerSingleton.Instance.Width / 2)
        {
            MoveHand(leftHandObj, playerPos.x, currLeftHandPos.y,ref currLeftHandPos);
            MoveHand(rightHandObj,playerPos.x+1,currLeftHandPos.y,ref currRightHandPos);
            
            isPlayerOnLeft = true;
            attack1yPos = currLeftHandPos.y;
        }
        else
        {
            MoveHand(rightHandObj,playerPos.x, currRightHandPos.y,ref currRightHandPos);
            MoveHand(leftHandObj, currRightHandPos.x-1, currRightHandPos.y,ref currLeftHandPos);
            
            attack1yPos = currRightHandPos.y;
        }
    }
    
    private Task MoveHand(GameObject handObj, int x, int y, ref int2 currentHandPos)
    {
        int2 newPos = new int2(x, y);
        currentHandPos = newPos;
        return DoMove(handObj, newPos);
    }
    
    private Task DoMove(GameObject obj, int2 pos)
    {
        return TileManagerSingleton.Instance.MoveObjectToTile(pos,obj);
    }

    private async void DoHeadVerticalAttack()
    {
        List<int2> attackList = new List<int2>();

        while (currHeadPos.y >= 0)
        {
            int2 targetPos = new int2(currHeadPos.x, currHeadPos.y--);
            if (CheckLimit(targetPos))
            {
                attackList.Add(targetPos);
            }
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);

        currHeadPos = attackList[^1];
        
        await TriggerVerticalAttackAnimation(attackList[^1],headObj);
    }

    private async void DoVerticalAttack(GameObject obj, int2 pos,bool value)
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
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
        
        if (value) currLeftHandPos = attackList[^1];
        else currRightHandPos = attackList[^1];
        
        await TriggerVerticalAttackAnimation(attackList[^1],obj);
    }

    private async void DoHorizontalAttack(GameObject obj, int2 pos,bool isLeft)
    {
        List<int2> attackList = new List<int2>();
        int direction = isLeft ? 1 : -1;
        for (int i = pos.x; i >= 0 && i < GameManagerSingleton.Instance.Width; i += direction)
        {
            int2 targetPos = new int2(i, pos.y);
            if (CheckLimit(targetPos)) attackList.Add(targetPos);
        }
        
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
        
        if (isLeft) currLeftHandPos = attackList[^1];
        else currRightHandPos = attackList[^1];
        
        await TriggerVerticalAttackAnimation(attackList[^1],obj);
    }

    private void DoAOEAttackBack(GameObject obj, int2 pos, bool isLeft)
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
        TileManagerSingleton.Instance.AddObjectToTile(tmp,obj);
        
        if (isLeft) currLeftHandPos = pos;
        else currRightHandPos = pos;
        TileManagerSingleton.Instance.AddObjectToTile(pos,obj);
    }
    
    private void DoAOEAttack(GameObject obj, int2 pos,bool isLeft)
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
        
        // add animation
        TileManagerSingleton.Instance.AddObjectToTile(tmp,obj);
    }

    private Task TriggerVerticalAttackAnimation(int2 targetPos, GameObject obj)
    {
        var TileManager = TileManagerSingleton.Instance;
        TileManager.TileList[TileManager.GetIndexPos(targetPos)].SetObjectToTile(obj);
        Transform currTrans = obj.GetComponent<Transform>();

        Sequence timeline = DOTween.Sequence();
        timeline.Insert(0, currTrans.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.Linear));
        timeline.Insert(0, currTrans.DOScale(0.75f, 0.15f));
        timeline.Insert(0.3f, currTrans.DOScale(1f, 0.15f));
        return timeline.Play().AsyncWaitForCompletion();
    }
    
    private bool CheckLimit(int2 targetPos)
    {
        return targetPos.x >= 0 && targetPos.x < GameManagerSingleton.Instance.Width && targetPos.y >= 0 && targetPos.y < GameManagerSingleton.Instance.Height;
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