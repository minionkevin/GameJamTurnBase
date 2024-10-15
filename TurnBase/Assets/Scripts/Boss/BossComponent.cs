using System.Collections.Generic;
using System.Threading.Tasks;
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
    
    public async void DoAttack1Pre()
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

        // 从左往右
        // Debug.Log("prev");
        // await Task.Delay(3000);
        //
        // Debug.Log("1");
        // // 第一次劈，玩家在左边，左手先劈
        // DoAttack1Step1(isPlayerOnLeft);
        // await Task.Delay(3000);
        //
        // Debug.Log("2");
        // // 第二次劈，左手移动，右手劈
        // DoAttack1Step2(true);
        //
        // await Task.Delay(3000);
        // Debug.Log("3");
        // // 第三次劈，右手移动，左手劈
        // DoAttack1Step2(false);
        //
        // await Task.Delay(3000);
        // Debug.Log("4");
        // // 第四次劈，右手劈
        // DoAttack1Step3(false);
        //
        // await Task.Delay(3000);
        // Debug.Log("5");
        // // 第五次劈，左手劈
        // DoAttack1Step3(true);
        
        
        // 从右往左
        // Debug.Log("prev");
        // await Task.Delay(3000);
        //
        // Debug.Log("1");
        // // 第一次劈，玩家在左边，左手先劈
        // DoAttack1Step1(isPlayerOnLeft);
        // await Task.Delay(3000);
        //
        // Debug.Log("2");
        // // 第二次劈，左手移动，右手劈
        // DoAttack1Step2(false);
        //
        // await Task.Delay(3000);
        // Debug.Log("3");
        // // 第三次劈，右手移动，左手劈
        // DoAttack1Step2(true);
        //
        // await Task.Delay(3000);
        // Debug.Log("4");
        // // 第四次劈，右手劈
        // DoAttack1StepRight(true);
        //
        // await Task.Delay(3000);
        // Debug.Log("5");
        //
        // // 第五次劈，左手劈
        // DoAttack1StepRight(false);
    }

    public void DoAttack1Step1(bool value)
    {
        if(value) DoVerticalAttack(leftHandObj, currLeftHandPos,true);
        else DoVerticalAttack(rightHandObj,currRightHandPos,false);
    }

    public void DoAttack1Step2(bool value)
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

    private void DoMove(GameObject obj, int2 pos)
    {
        TileManagerSingleton.Instance.MoveObjectToTile(pos,obj);
    }

    private void DoVerticalAttack(GameObject obj, int2 pos,bool value)
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
        
        // 用动画来动手
        if (value) currLeftHandPos = attackList[^1];
        else currRightHandPos = attackList[^1];
        
        TileManagerSingleton.Instance.MoveObjectToTile(attackList[^1],obj);
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
}
