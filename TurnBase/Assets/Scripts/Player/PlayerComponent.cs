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
        GameManagerSingleton.Instance.Boss.CheckForDamage(2, attackList);
        
        // 没有动画，触发粒子特效
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
    
    public void ResetPlayerState()
    {
        isJumping = false;
        isUnderProtected = false;
        // (状态对应的动画，也一并重置)
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
        shouldDamage = true;
        damageAmount = _damage;
    }



    // ------------------------------------------

    // placeholder animation
    private async void TakeDamageAnimation()
    {
        Color tmp = PlayerSprite.color;
        Sequence timeline = DOTween.Sequence();
        timeline.Insert(0, PlayerSprite.DOColor(Color.red, 0.15f));
        timeline.Insert(0.15f, PlayerSprite.DOColor(Color.white, 0.15f));
        await timeline.SetLoops(3).Play().AsyncWaitForCompletion();
        PlayerSprite.color = tmp;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Boss" && shouldDamage)
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

    public const int NULL = 100;     // 空指令,用于补全玩家未输入指令。 
    public const int END = 101;      // 结束指令，在玩家指令队列的最后，默认添加此指令。
}