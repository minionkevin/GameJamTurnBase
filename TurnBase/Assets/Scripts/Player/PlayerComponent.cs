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
        ResetPlayerState();
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

    /// <summary>
    /// 跳跃
    /// </summary>
    public void DoJump()
    {
        isJumping = true;
        // 跳跃动画
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

    public void OnTakeDamage(int _damage)
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


    private bool CheckLimit(int2 newPos)
    {
        return newPos.x >= 0 && newPos.x < GameManagerSingleton.Instance.Width && newPos.y >= 0 && newPos.y < GameManagerSingleton.Instance.Height;
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