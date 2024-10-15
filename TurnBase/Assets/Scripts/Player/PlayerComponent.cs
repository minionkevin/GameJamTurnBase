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

    
    public void Setup(int2 playerPos)
    {
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
        // 攻击力和攻击范围应该读表
        List<int2> list = new List<int2>() {new int2(-2,0), new int2(-1, 0), new int2(0, 0), new int2(1, 0), new int2(2, 0)};
        int damage = 2;

        for (int i = 0; i < list.Count; i++)
        {
            // check boss head or hand on the pos
            // currPlayerPos + list[i]
        }
    }

    /// <summary>
    /// 处理十字重攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public void DoCrossAttack()
    {
        // 攻击力和攻击范围应该读表
        List<int2> list = new List<int2>() { new int2(0, 1), new int2(-1, 0), new int2(0, 0), new int2(1, 0) };
        int damage = 4;

        // check boss head or hand on the pos
        // currPlayerPos + list[i]
    }

    /// <summary>
    /// 加血
    /// </summary>
    public void DoHeal()
    {
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

    public void OnTakgeDamage(int _damage)
    {
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

