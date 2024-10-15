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

    private int yPos;

    public void Setup(int2 bossHeadPos, int2 bossLeftHandPos, int2 bossRightHandPos,GameObject head, GameObject leftHand, GameObject rightHand)
    {
        currHeadPos = bossHeadPos;
        currLeftHandPos = bossLeftHandPos;
        currRightHandPos = bossRightHandPos;
        headObj = head;
        leftHandObj = leftHand;
        rightHandObj = rightHand;
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
    
    public void DoAttack1Pre()
    {
        int2 playerPos = GameManagerSingleton.Instance.Player.GetPlayerPos(); 
        // 判断玩家在左右
        // todo 简化
        if (playerPos.x < GameManagerSingleton.Instance.Width / 2)
        {
            int2 leftHandPos = new int2(playerPos.x, currLeftHandPos.y);
            DoMove(leftHandObj, leftHandPos);
            currLeftHandPos = leftHandPos;
            int2 rightHandPos = new int2(playerPos.x + 1, currLeftHandPos.y);
            DoMove(rightHandObj,rightHandPos);
            currRightHandPos = rightHandPos;
            isPlayerOnLeft = true;
            yPos = currLeftHandPos.y;
        }
        else
        {
            int2 rightHandPos = new int2(playerPos.x, currRightHandPos.y);
            DoMove(rightHandObj, rightHandPos);
            currRightHandPos = rightHandPos;
            int2 leftHandPos = new int2(rightHandPos.x - 1, currRightHandPos.y);
            DoMove(leftHandObj,leftHandPos);
            currLeftHandPos = leftHandPos;
            yPos = currRightHandPos.y;
        }
    }

    public void DoAttack1Step1()
    {
        if(isPlayerOnLeft) DoVerticalAttack(leftHandObj, currLeftHandPos,true);
        else DoVerticalAttack(rightHandObj,currRightHandPos,false);
    }

    public void HandleVerticalAttackAndMove(bool isFirstTime)
    {
        bool tmp = false;
        switch (isPlayerOnLeft)
        {
            case true when isFirstTime:
                tmp = true;
                break;
            case true when !isFirstTime:
            case false when isFirstTime:
                tmp = false;
                break;
            case false when !isFirstTime:
                tmp = true;
                break;
        }
        
        DoAttack1Step2(tmp);
    }

    public void HandleVerticalMove(bool isFirstTime)
    {
        bool tmp = false;
        switch (isFirstTime)
        {
            case true when isPlayerOnLeft:
                tmp = false;
                break;
            case false when isPlayerOnLeft:
            case true when !isPlayerOnLeft:
                tmp = true;
                break;
            case false when !isPlayerOnLeft:
                tmp = false;
                break;
        }
        if (isPlayerOnLeft) DoAttack1StepLeft(tmp);
        else DoAttack1StepRight(tmp);
    }

    private void DoAttack1Step2(bool value)
    {
        if (value)
        {
            int2 newPos = new int2(isPlayerOnLeft?currRightHandPos.x + 1:currRightHandPos.x - 1, yPos);
            DoVerticalAttack(rightHandObj,currRightHandPos,false);
            DoMove(leftHandObj,newPos);
            currLeftHandPos = newPos;
        }
        else
        {
            int2 newPos = new int2(isPlayerOnLeft?currLeftHandPos.x + 1:currLeftHandPos.x - 1,yPos);
            DoVerticalAttack(leftHandObj, currLeftHandPos,true);
            DoMove(rightHandObj,newPos);
            currRightHandPos = newPos;
        }
    }

    public void DoAttack1StepLeft(bool value)
    {
        if (value)
        {
            DoVerticalAttack(leftHandObj, currLeftHandPos,true);
        }
        else
        {
            int2 leftHandPos = new int2(currLeftHandPos.x, yPos);
            DoMove(leftHandObj,leftHandPos);
            currLeftHandPos = leftHandPos;
            DoVerticalAttack(rightHandObj,currRightHandPos,false);
        }
    }
    
    public void DoAttack1StepRight(bool value)
    {
        if (value)
        {
            int2 newPos = new int2(currRightHandPos.x, yPos);
            DoMove(rightHandObj,newPos);
            currRightHandPos = newPos;
            DoVerticalAttack(leftHandObj, currLeftHandPos,true);
            
        }
        else
        {
            DoVerticalAttack(rightHandObj, currRightHandPos,false);
        }
    }

    public void DoAttack2Pre()
    {
        int2 leftPos = new int2(1,GameManagerSingleton.Instance.Height-2);
        DoMove(leftHandObj,leftPos);
        currLeftHandPos = leftPos;
        int2 rightPos = new int2(GameManagerSingleton.Instance.Width-2,GameManagerSingleton.Instance.Height-2);
        DoMove(rightHandObj,rightPos);
        currRightHandPos = rightPos;
        int2 headPos = new int2(GameManagerSingleton.Instance.Width/2,GameManagerSingleton.Instance.Height-1);
        DoMove(headObj,headPos);
        currHeadPos = headPos;
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