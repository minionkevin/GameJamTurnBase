using System.Collections.Generic;
using System.Threading.Tasks;
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
        ResetPlayerState();
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dir"></param>
    public Task DoMove(int2 dir)
    {
        int2 newPos = dir + currPlayerPos;
        if (!CheckLimit(newPos)) return null;
        currPlayerPos = newPos;
        return TileManagerSingleton.Instance.MoveObjectToTile(currPlayerPos, gameObject);
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
    /// todo remake
    public void DoJump()
    {
        isJumping = true;
        // domoveup
        
        
        // 1.开始之前记录当前位置到变量
        // 2.DoMove把玩家移到y-1位置
        // 3.处理下一个指令
        
        // 4.每次处理指令之前检查上一个是不是跳跃
        // 5.是的话就等当前指令做完用DoMove（y-1）
    }

    /// <summary>
    /// 处理横向轻攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public void DoHorizontalAttack()
    {
        List<int2> attackList = new List<int2>();
        for (int i = -2; i < 3; i++)
        {
            attackList.Add(new int2(currPlayerPos.x + i,currPlayerPos.y));
        }
        GameManagerSingleton.Instance.Boss.CheckForDamage(2, attackList);
        
        // 没有动画，触发粒子特效
    }

    /// <summary>
    /// 处理十字重攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public void DoCrossAttack()
    {
        List<int2> attackList = new List<int2>() { new int2(currPlayerPos.x, currPlayerPos.y + 1), new int2(currPlayerPos.x -1, currPlayerPos.y), currPlayerPos, new int2(currPlayerPos.x+1, currPlayerPos.y) };
        GameManagerSingleton.Instance.Boss.CheckForDamage(4, attackList);
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

    /// <summary>
    /// 重置状态
    /// </summary>
    public void ResetPlayerState()
    {
        isJumping = false;
        isUnderProtected = false;
        // (状态对应的动画，也一并重置)
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
}