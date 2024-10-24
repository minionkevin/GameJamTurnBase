using System.Collections;
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

    public SpriteRenderer PlayerSprite;
    public bool IsLastJump;
    public bool IsLastDefense;

    private PlayerInputType preInputType;
    private int healMax;

    private int damageAmount;
    private bool shouldDamage;
    private bool laserDamage;
    
    public void Setup(int2 playerPos)
    {
        healMax = 2;
        currPlayerPos = playerPos;
        // 初始化时，重置到默认状态
        isJumping = false;
        IsLastDefense = false;
        IsLastJump = false;
    }

    public void Reset()
    {
        int2 startPos = GameManagerSingleton.Instance.PlayerStartPos;
        if (!currPlayerPos.Equals(startPos))
        {
            TileManagerSingleton.Instance.MoveObjectToTile(startPos, gameObject);
        }
        Setup(startPos);
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
    public async void DoJump()
    {
        // 跳跃动画
        isJumping = true;
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("JumpTrigger");
        await DoMove(new int2(0, 1));
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("JumpIdleTrigger");

        IsLastJump = true;
    }
    

    public void HandleLastJump()
    {
        if (!IsLastJump) return;
        DoMove(new int2(0, -1));
        IsLastJump = false;
    }

    public void HandleLastDefense()
    {
        if (!IsLastDefense || !laserDamage) return;
        IsLastDefense = false;

        List<int2> attackList = new List<int2>();
        int index = 0;
        for (int i = 0; i < GameManagerSingleton.Instance.Height; i++)
        {
            for (int j = -index; j < index+1; j++)
            {
                int2 targetPos = new int2(currPlayerPos.x + j, currPlayerPos.y + i);
                if (!CheckLimit(targetPos)) continue;
                attackList.Add(targetPos);
            }
            index += 1;
        }
        GameManagerSingleton.Instance.Boss.CheckForDamage(4, attackList);
    }

    /// <summary>
    /// 处理横向轻攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public Task DoHorizontalAttack(int damage = 2)
    {
        // 正常攻击
        List<int2> attackList = new List<int2>();
        for (int i = -2; i < 3; i++)
        {
            int2 newPos = new int2(currPlayerPos.x + i, currPlayerPos.y);
            if (!CheckLimit(newPos)) continue;
            attackList.Add(newPos);
        }
        GameManagerSingleton.Instance.Boss.CheckForDamage(3, attackList);
        
        return HandleAnimation( "SwordTrigger","playerASword");
    }

    private Task HandleAnimation(string triggerName, string animationNameA)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine( PlayAnimationAndWait(triggerName,animationNameA, tcs) );
        return tcs.Task;
    }
    
    IEnumerator PlayAnimationAndWait(string triggerName,string animationName, TaskCompletionSource<bool> tcs)
    {
        var animator = GameManagerSingleton.Instance.PlayerAnimator;
        animator.SetTrigger(triggerName);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        tcs.SetResult(true);
    }
    

    /// <summary>
    /// 处理十字重攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public Task DoCrossAttack(int damage = 4)
    {
        List<int2> attackList = new List<int2>();
        
        for (int i = 0; i < 3; i++)
        {
            int2 newPos = new int2(currPlayerPos.x, currPlayerPos.y + i);
            if (!CheckLimit(newPos)) continue;
            attackList.Add(newPos);
        }
        
        GameManagerSingleton.Instance.Boss.CheckForDamage(damage, attackList);

        return HandleAnimation("HammerTrigger", "playerAHammer");
    }

    /// <summary>
    /// 加血
    /// </summary>
    public Task DoHeal()
    {
        healMax--;
        if (healMax <= 0) return null;
        GameManagerSingleton.Instance.PlayerHp_UI.OnGetRecovery(GameManagerSingleton.Instance.playerStartHp);
        return HandleAnimation("HealTrigger", "playerAHeal");
    }

    /// <summary>
    /// 护盾
    /// </summary>
    public Task DoProtected()
    {
        return HandleAnimation("ShieldTrigger", "playerAShield");
    }
    
    public void ResetPlayerState()
    {
        isJumping = false;
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

    private void OnTakeDamage(int _damage)
    {
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
        if (other.gameObject.CompareTag("Laser") && shouldDamage)
        {
            if (IsLastDefense)
            {
                laserDamage = true;
                return;
            }
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
}