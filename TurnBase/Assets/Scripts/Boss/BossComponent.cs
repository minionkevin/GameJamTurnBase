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
                PerformAttack(leftHandObj, currLeftHandPos, true);
                break;
            case 2:
                PerformAttack(rightHandObj, currRightHandPos, false);
                MoveHand(leftHandObj, 2, GameManagerSingleton.Instance.Height - 2, ref currLeftHandPos);
                break;
            case 3:
                PerformAttack(leftHandObj, currLeftHandPos, true);
                MoveHand(rightHandObj, GameManagerSingleton.Instance.Width - 3, GameManagerSingleton.Instance.Height - 2, ref currRightHandPos);
                break;
            case 4:
                PerformAttack(rightHandObj, currRightHandPos, false);
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
    
    private void PerformAttack(GameObject handObj, int2 handPos, bool isLeft)
    {
        DoAOEAttack(handObj, handPos, isLeft);
    }

    private void MoveHand(GameObject handObj, int x, int y, ref int2 currentHandPos)
    {
        int2 newPos = new int2(x, y);
        DoMove(handObj, newPos);
        currentHandPos = newPos;
    }
    
    private void DoMove(GameObject obj, int2 pos)
    {
        TileManagerSingleton.Instance.MoveObjectToTile(pos,obj);
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
    
    public void DoAOEAttack(GameObject obj, int2 pos,bool isLeft)
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


public class BossInputType
{
    // 连续下劈
    public const int ATTACK1 = 0;
    // 左右双掌
    public const int ATTACK2 = 1;
    // 全屏激光
    public const int ATTACK3 = 2;
    // 地面清扫
    public const int ATTACK4 = 3;
    // 范围拍击
    public const int ATTACK5 = 4;

    public const int ATTACK10 = 10;
    public const int ATTACK11 = 11;
    public const int ATTACK12 = 12;
    public const int ATTACK13 = 13;
    public const int ATTACK14 = 14;
    public const int ATTACK15 = 15;

    public const int ATTACK20 = 20;
    public const int ATTACK21 = 21;
    public const int ATTACK22 = 22;
    public const int ATTACK23 = 23;
    public const int ATTACK24 = 24;
    public const int ATTACK25 = 25;
}