using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
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

    public SpriteRenderer PlayerSprite;

    private PlayerInputType preInputType;
    private int healMax;

    private int damageAmount;
    private bool shouldDamage;
    
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
    public Task DoMove(int2 dir)
    {
        int2 newPos = dir + currPlayerPos;
        if (!CheckLimit(newPos)) return null;
        currPlayerPos = newPos;
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger(dir.Equals(new int2(-1, 0)) ? "MoveLeftTrigger" : "MoveRightTrigger");
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
        // 跳跃动画
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("JumpTrigger");
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
            int2 newPos = new int2(currPlayerPos.x + i, currPlayerPos.y);
            if (!CheckLimit(newPos)) continue;
            attackList.Add(newPos);
        }
        GameManagerSingleton.Instance.Boss.CheckForDamage(2, attackList);
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("SwordTrigger");
    }

    /// <summary>
    /// 处理十字重攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public void DoCrossAttack(int damage = 4)
    {
        List<int2> attackList = new List<int2>();
        
        int2 pos = new int2(currPlayerPos.x, currPlayerPos.y + 1);
        if (CheckLimit(pos)) attackList.Add(pos);
        pos = new int2(currPlayerPos.x - 1, currPlayerPos.y);
        if (CheckLimit(pos)) attackList.Add(pos);
        pos = new int2(currPlayerPos.x + 1, currPlayerPos.y);
        if (CheckLimit(pos)) attackList.Add(pos);
        attackList.Add(currPlayerPos);
        
        GameManagerSingleton.Instance.Boss.CheckForDamage(damage, attackList);
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("HammerTrigger");
    }

    /// <summary>
    /// 加血
    /// </summary>
    public void DoHeal()
    {
        healMax--;
        if (healMax <= 0) return;
        GameManagerSingleton.Instance.PlayerHp_UI.OnGetRecovery(2);
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("HealTrigger");
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
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("ShieldTrigger");
        // Anim

        // VFX

        // Audio
    }
    
    public void ResetPlayerState()
    {
        isJumping = false;
        isUnderProtected = false;
        // (状态对应的动画，也一并重置)
    }

    public void CheckForDamage(List<int2> attackList,int value)
    {
        foreach (var pos in attackList)
        {
            TileManagerSingleton.Instance.ChangeTileColorPlayer(pos);
            if (currPlayerPos.Equals(pos)) OnTakeDamage(value);
        }
    }

    public void CheckForDamageLaser(List<int2> attackList,int value)
    {
        foreach (var pos in attackList)
        {
            if (!currPlayerPos.Equals(pos)) continue;
            GameManagerSingleton.Instance.PlayerHp_UI.OnTakeDamage(value);
            TakeDamageAnimation();
        }
    }

    private void OnTakeDamage(int _damage)
    {
        if (isJumping) return;

        if (isUnderProtected) _damage /= 2;
        
        // Anim

        // VFX

        // Audio
        shouldDamage = true;
        damageAmount = _damage;
    }
    
    private void TakeDamageAnimation()
    {
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("DamageTrigger");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss") && shouldDamage)
        {
            GameManagerSingleton.Instance.PlayerHp_UI.OnTakeDamage(damageAmount);
            TakeDamageAnimation();
        }
        shouldDamage = false;
        damageAmount = 0;
    }
    

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