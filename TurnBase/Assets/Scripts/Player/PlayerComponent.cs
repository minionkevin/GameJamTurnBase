using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    // 每次玩家移动需更新
    [HideInInspector]
    public int2 currPlayerPos;
  
    [HideInInspector]
    public bool isJumping;          // 玩家跳跃的状态
    [HideInInspector]
    public bool isUnderProtected;   // 护盾开启的状态


    private PlayerInputType preInputType;
    private int healMax;
    
    public void Setup(int2 playerPos)
    {
        healMax = 4;
        currPlayerPos = playerPos;
        // 初始化时，重置到默认状态
        isJumping = false;
        isUnderProtected = false;
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dir"></param>
    public void DoMove(int2 dir)
    {        
        int2 newPos = dir + currPlayerPos;
        if (!CheckLimit(newPos)) return;
        currPlayerPos = newPos;
        TileManagerSingleton.Instance.MoveObjectToTile(currPlayerPos, gameObject);
    }
    

    /// <returns>
    /// 玩家位置
    /// </returns>
    public int2 GetPlayerPos()
    {
        return currPlayerPos;
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    public void DoJump()
    {
        isJumping = true;
        // 跳跃动画

        DoMove(new int2(0, 1));

    }

    /// <summary>
    /// 处理横向轻攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public void DoHorizontalAttack(int damage = 2)
    {
        // 正常攻击
        List<int2> attackList = new List<int2>();
        for (int i = -2; i < 3; i++)
        {
            attackList.Add(new int2(currPlayerPos.x + i, currPlayerPos.y));
        }
        GameManagerSingleton.Instance.Boss.CheckForDamage(damage, attackList);
    }

    /// <summary>
    /// 处理十字重攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public void DoCrossAttack(int damage = 4)
    {
        List<int2> attackList = new List<int2>() { new int2(currPlayerPos.x, currPlayerPos.y + 1), new int2(currPlayerPos.x -1, currPlayerPos.y), currPlayerPos, new int2(currPlayerPos.x+1, currPlayerPos.y) };
        GameManagerSingleton.Instance.Boss.CheckForDamage(damage, attackList);
    }

    /// <summary>
    /// 加血
    /// </summary>
    public void DoHeal()
    {
        healMax--;
        if (healMax <= 0) return;
        GameManagerSingleton.Instance.PlayerHp_UI.OnGetRecovery(2);
        // Anim

        // VFX

        // Audio
    }

    /// <summary>
    /// 护盾
    /// </summary>
    public void DoProtected()
    {
        isUnderProtected = true;
        // Anim

        // VFX

        // Audio
    }

    /// <summary>
    /// 跳跃攻击
    /// </summary>
    public void DoJumpAttack(int attackType)
    {
        if (attackType.Equals(PlayerInputType.ATTACK1))
        {
            DoHorizontalAttack();
        }
        else if (attackType.Equals(PlayerInputType.ATTACK2))
        {
            DoCrossAttack();
        }
    }

    /// <summary>
    /// 玩家轮次结束后，默认添加的一个状态
    /// </summary>
    public void DoActionEnd()
    {
        isUnderProtected = false;
    }

    /// <summary>
    /// 没有输入指令时，执行的一条指令（占位符，为了指令和Boss对齐）
    /// </summary>
    public void DoNothing()
    {
        // 
        Debug.Log("没输入指令，跳过回合");
    }

    public void CheckForDamage(List<int2> attackList,int value)
    {
        foreach (var pos in attackList)
        {
            if (currPlayerPos.Equals(pos)) OnTakeDamage(value);
        }
    }

    private void OnTakeDamage(int _damage)
    {
        if (isJumping) return;

        if (isUnderProtected) _damage /= 2;
        
        // Anim

        // VFX

        // Audio

        GameManagerSingleton.Instance.PlayerHp_UI.OnTakeDamage(_damage);
    }



    // ------------------------------------------


    private bool CheckLimit(int2 targetPos)
    {
        return targetPos.x >= 0 && targetPos.x < GameManagerSingleton.Instance.Width && targetPos.y >= 0 && targetPos.y < GameManagerSingleton.Instance.Height;
    }

}

public class PlayerInputType
{
    public const int MOVEA = 0;
    public const int MOVED = 1;
    public const int ATTACK1 = 2;
    public const int ATTACK2 = 3;
    public const int DEFENSE = 4;
    public const int HEAL = 5;
    public const int JUMP = 6;
    public const int JUMPATTACK = 7;

    public const int NULL = 100;     // 空指令,用于补全玩家未输入指令。 
    public const int END = 101;      // 结束指令，在玩家指令队列的最后，默认添加此指令。
}