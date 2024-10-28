using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class BossComponent : MonoBehaviour
{
    public Material BossUnderDamageMaterial;
    
    private int2 currHeadPos;
    private int2 currLeftHandPos;
    private int2 currRightHandPos;

    private GameObject headObj;
    private GameObject leftHandObj;
    private GameObject rightHandObj;

    // 需要reset
    private bool isPlayerOnLeft;
    private int attack1yPos;

    private int width;
    private int height;

    private BossHandComponent leftHand;
    private BossHandComponent rightHand;

    AudioManager audioManager;

    public void Setup(int2 bossHeadPos, int2 bossLeftHandPos, int2 bossRightHandPos,GameObject head, GameObject leftHand, GameObject rightHand,AudioManager am)
    {
        currHeadPos = bossHeadPos;
        currLeftHandPos = bossLeftHandPos;
        currRightHandPos = bossRightHandPos;
        headObj = head;
        leftHandObj = leftHand;
        rightHandObj = rightHand;
        width = GameManagerSingleton.Instance.Width;
        height = GameManagerSingleton.Instance.Height;

        this.leftHand = leftHand.GetComponent<BossHandComponent>();
        this.rightHand = rightHand.GetComponent<BossHandComponent>();

        audioManager = am;
    }

    #region attack1-连续下劈
    public void DoAttack1Step(int step)
    {
        switch (step)
        {
            case 1:
                GameManagerSingleton.Instance.BossLeftAnimator.Play("bossChop");
                GameManagerSingleton.Instance.BossRightAnimator.Play("bossChop");
                DoAttackSingleHand();
                break;
            case 2:
                DoAttackAndMoveHand(Attack1StepCheck(true),true);
                break;
            case 3:
                DoAttackAndMoveHand(Attack1StepCheck(false),true);
                break;
            case 4:
                DoAttackAndMoveHand(Attack1StepCheck(true),true);
                break;
            case 5:
                DoAttackAndMoveHand(Attack1StepCheck(false));
                break;

        }
    }
    
    public void DoAttack1Pre()
    {
        GameManagerSingleton.Instance.BossLeftAnimator.Play("bossChopWarning");
        GameManagerSingleton.Instance.BossRightAnimator.Play("bossChopWarning");

        SetupHandsForAttack1();
        MoveObject(headObj, new int2(isPlayerOnLeft ? width - 2 : 1, height - 1),ref currHeadPos);
    }

    private void SetupHandsForAttack1()
    {
        int2 playerPos = GameManagerSingleton.Instance.Player.GetPlayerPos(); 
        
        if (playerPos.x < width / 2)
        {
            MoveObject(leftHandObj, 1, height-2,ref currLeftHandPos);
            MoveObject(rightHandObj,2,height-2,ref currRightHandPos);
            
            isPlayerOnLeft = true;
            attack1yPos = currLeftHandPos.y;
        }
        else
        {
            MoveObject(rightHandObj,width-2, height-2,ref currRightHandPos);
            MoveObject(leftHandObj, width-3, height-2,ref currLeftHandPos);
            
            attack1yPos = currRightHandPos.y;
        }
    }

    private async void DoAttackSingleHand()
    {
        if (isPlayerOnLeft)
        {
            await DoVerticalAttack(leftHandObj, currLeftHandPos,true);
            leftHand.PlayChop();
        }
        else
        {
            await DoVerticalAttack(rightHandObj,currRightHandPos,false);
            rightHand.PlayChop();
        }
        audioManager.AudioUI.clip = audioManager.AudioDic["巨石撞击地面"];
        audioManager.AudioUI.Play();
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
    

    private async void DoAttackAndMoveHand(bool value,bool isMoveBack = false)
    {
        if (value)
        {
            await DoVerticalAttack(rightHandObj,currRightHandPos,false);
            if(isMoveBack)MoveObject(leftHandObj, isPlayerOnLeft ? currRightHandPos.x + 1 : currRightHandPos.x - 1, attack1yPos, ref currLeftHandPos);
            rightHand.PlayChop();
        }
        else
        {
            await DoVerticalAttack(leftHandObj, currLeftHandPos,true);
            if(isMoveBack)MoveObject(rightHandObj,isPlayerOnLeft?currLeftHandPos.x + 1:currLeftHandPos.x - 1,attack1yPos,ref currRightHandPos);
            leftHand.PlayChop();
        }
        audioManager.AudioUI.clip = audioManager.AudioDic["巨石撞击地面"];
        audioManager.AudioUI.Play();
    }
    #endregion

    #region attack2-左右双掌 
    public void DoAttack2Pre()
    {
        GameManagerSingleton.Instance.BossLeftAnimator.Play("bossPalmWarning");
        GameManagerSingleton.Instance.BossRightAnimator.Play("bossPalmWarning");
        
        MoveObject(leftHandObj, 2, height - 2, ref currLeftHandPos);
        MoveObject(rightHandObj,width-3,height-2,ref currRightHandPos);
        MoveObject(headObj,width/2,height-1,ref currHeadPos);
    }
    public async void DoAttack2Step(int step)
    {
        switch (step)
        {
            case 1:
                GameManagerSingleton.Instance.BossLeftAnimator.Play("bossPalm");
                GameManagerSingleton.Instance.BossRightAnimator.Play("bossPalm");
                
                await DoAOEAttackBack(leftHandObj, currLeftHandPos, true,false);
                break;
            case 2:
                await MoveObject(leftHandObj, 1, height - 2, ref currLeftHandPos);
                await DoAOEAttackBack(leftHandObj, currLeftHandPos, true,false);
                break;
            case 3:
                MoveObject(leftHandObj, 1, height - 2, ref currLeftHandPos);
                await DoAOEAttackBack(rightHandObj, currRightHandPos, false,false);
                break;
            case 4:
                await MoveObject(rightHandObj, width - 2, height - 2, ref currRightHandPos);
                await DoAOEAttackBack(rightHandObj, currRightHandPos, false,false);
                break;
            case 5:
                MoveObject(rightHandObj, width - 2, height - 2, ref currRightHandPos);
                await DoHeadVerticalAttack();
                audioManager.AudioUI.clip = audioManager.AudioDic["巨石撞击地面"];
                audioManager.AudioUI.Play();
                await MoveObject(headObj, currHeadPos.x, 1, ref currHeadPos);
                break;
        }
    }
    #endregion

    #region attack3-全屏激光
    public async void DoAttack3Step(int step)
    {
        switch (step)
        {
            case 3:
                DoAttack3MoveHand();
                break;
            case 4:
                DoAttack3HandAttack(false);
                break;
            case 5:
                break;
        }
    }
    
    public Task DoAttack3Pre()
    {
        GameManagerSingleton.Instance.BossLeftAnimator.Play("bossFist");
        GameManagerSingleton.Instance.BossRightAnimator.Play("bossFist");

        audioManager.AudioUI.clip = audioManager.AudioDic["部件蓄力"];
        audioManager.AudioUI.Play();

        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(PlayAnimationAndWait("WarningTrigger", "bossWarning", tcs));
        SetupBossStartPos(new int2(1, height - 2),
            new int2(width-2,height-2),
            new int2(width/2, 1));
        return tcs.Task;

    }

    public Task DoAttack3Step1(bool isLeft)
    {
        List<int2> attackList = new List<int2>();
        int2 targetPos = new int2(width / 2, 0);
        foreach (var tile in TileManagerSingleton.Instance.TileList)
        {
            int2 pos = tile.GetTilePos();
            if(!pos.Equals(targetPos)) attackList.Add(pos);
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,2);
    
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(isLeft ? PlayAnimationAndWait("LeftLaserTrigger", "bossLaserL", tcs) : PlayAnimationAndWait("RightLaserTrigger", "bossLaserR", tcs));

        audioManager.AudioUI.clip = audioManager.AudioDic["激光扫射"];
        audioManager.AudioUI.Play();

        return tcs.Task;
    }
    
    IEnumerator PlayAnimationAndWait(string triggerName,string animationName, TaskCompletionSource<bool> tcs)
    {
        var animator = GameManagerSingleton.Instance.BossAnimator;
        animator.SetTrigger(triggerName);
        if (GameManagerSingleton.Instance.IsPlayerDie)
        {
            tcs.SetResult(true);
            yield break;
        }
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        tcs.SetResult(true);
    }

    private async void DoAttack3MoveHand()
    {
        await Task.WhenAll(MoveObject(rightHandObj, currRightHandPos.x - 1, currHeadPos.y + 1, ref currRightHandPos),
            MoveObject(leftHandObj, currLeftHandPos.x + 1, currHeadPos.y + 1, ref currLeftHandPos));
    }

    private async void DoAttack3HandAttack(bool needBack)
    {
        await Task.WhenAll(DoAOEAttackBack(leftHandObj, currLeftHandPos, true,needBack), DoAOEAttackBack(rightHandObj, currRightHandPos, false,needBack));
    }
    #endregion
    
    # region attack4-地面清扫
    public Task DoAttack4Pre()
    {
        GameManagerSingleton.Instance.BossLeftAnimator.Play("bossChopWarning");
        GameManagerSingleton.Instance.BossRightAnimator.Play("bossChopWarning");
        
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(PlayAnimationAndWait("WarningTrigger", "bossWarning", tcs));
        SetupBossStartPos(new int2(1, height - 2),
            new int2(width-2,height-2),
               new int2(width/2, height - 2));
        return tcs.Task;
    }

    public async void DoAttack4Step(int step)
    {
        switch (step)
        {
            case 1:
                GameManagerSingleton.Instance.BossLeftAnimator.Play("bossChop");
                GameManagerSingleton.Instance.BossRightAnimator.Play("bossChop");
                await DoAttack4Step1(true);
                break;
            case 2:
                await MoveObject(headObj,currHeadPos.x,currHeadPos.y-1,ref currHeadPos);
                await DoAttack4Step1(false);
                break;
            case 3:
                await MoveObject(leftHandObj, currLeftHandPos.x, 0, ref currLeftHandPos);
                await DoHorizontalAttack(leftHandObj, currLeftHandPos, true);
                MoveObject(leftHandObj, width - 1, height - 1, ref currLeftHandPos);
                break;
            case 4:
                await MoveObject(rightHandObj, currRightHandPos.x, 0, ref currRightHandPos);
                await DoHorizontalAttack(rightHandObj, currRightHandPos, false);
                break;
            case 5:
                DoAttack4Step3();
                break;
        }
    }
    
    public Task DoAttack4Step1(bool isLeft)
    {
        // 激光
        List<int2> attackList = new List<int2>();
        int2 targetPos1 = new int2(width / 2, 0);
        int2 targetPos2 = new int2(width / 2, 1);
        int2 targetPos3 = new int2(width / 2-1, 0);
        int2 targetPos4 = new int2(width / 2+1, 0);
        
        foreach (var tile in TileManagerSingleton.Instance.TileList)
        {
            int2 pos = tile.GetTilePos();
            if (!pos.Equals(targetPos1) && !pos.Equals(targetPos2) && !pos.Equals(targetPos3) && !pos.Equals(targetPos4))
            {
                attackList.Add(pos);
            }
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,2);

        audioManager.AudioUI.clip = audioManager.AudioDic["激光扫射"];
        audioManager.AudioUI.Play();

        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(isLeft ? PlayAnimationAndWait("LeftLaserTrigger", "bossLaserL", tcs) : PlayAnimationAndWait("RightLaserTrigger", "bossLaserR", tcs));
        return tcs.Task;
        
        // 播放动画
        // 播放音效
    }
    
    private void DoAttack4Step3()
    {
        MoveObject(headObj, width/2, height - 1, ref currHeadPos);
        MoveObject(leftHandObj, currHeadPos.x-2, height - 2, ref currLeftHandPos);
        MoveObject(rightHandObj,currHeadPos.x+2,height-2,ref currRightHandPos);
    }
    #endregion

    #region attack5-范围拍击
    public void DoAttack5Pre()
    {
        GameManagerSingleton.Instance.BossLeftAnimator.Play("bossPalmWarning");
        GameManagerSingleton.Instance.BossRightAnimator.Play("bossPalmWarning");
        
        MoveObject(headObj, width / 2, height-1, ref currHeadPos);
        MoveHandToPlayerTop();
    }

    public async void DoAttack5Step1(int value = 1)
    {
        GameManagerSingleton.Instance.BossLeftAnimator.Play("bossPalm");
        GameManagerSingleton.Instance.BossRightAnimator.Play("bossPalm");
        if (isPlayerOnLeft)
        {
            int2 newPos = new int2(currRightHandPos.x + value, currRightHandPos.y);
            if (!CheckLimit(newPos)) return;
            if (!newPos.Equals(currRightHandPos)) await MoveObject(rightHandObj, newPos.x, newPos.y, ref currRightHandPos);
        }
        else
        {
            int2 newPos = new int2(currLeftHandPos.x - value, currLeftHandPos.y);
            if (!CheckLimit(newPos)) return;
            if (!newPos.Equals(currLeftHandPos)) await MoveObject(leftHandObj, newPos.x, newPos.y, ref currLeftHandPos);
        }
        await Task.WhenAll(DoAOEAttackBack(leftHandObj, currLeftHandPos, true), DoAOEAttackBack(rightHandObj, currRightHandPos, false));
    }

    public async void DoAttack5Step2()
    {
        MoveObject(rightHandObj, width - 2, height - 2, ref currRightHandPos);
        
        await DoHeadVerticalAttack(); 
        await MoveObject(headObj, currHeadPos.x, 1, ref currHeadPos);
    }

    #endregion
    
    
    private void SetupBossStartPos(int2 leftHandPos,int2 rightHandPos, int2 headPos)
    {
        MoveObject(headObj, headPos, ref currHeadPos);
        MoveObject(leftHandObj, leftHandPos, ref currLeftHandPos);
        MoveObject(rightHandObj, rightHandPos, ref currRightHandPos);
    }
    
    // dynamic移动手到玩家上方
    private void MoveHandToPlayerTop()
    {
        int2 playerPos = GameManagerSingleton.Instance.Player.GetPlayerPos(); 
        
        if (playerPos.x < width / 2)
        {
            MoveObject(leftHandObj, playerPos.x, height-2,ref currLeftHandPos);
            MoveObject(rightHandObj,playerPos.x+1,height-2,ref currRightHandPos);
            
            isPlayerOnLeft = true;
            attack1yPos = currLeftHandPos.y;
        }
        else
        {
            MoveObject(rightHandObj,playerPos.x, height-2,ref currRightHandPos);
            MoveObject(leftHandObj, currRightHandPos.x-1, height-2,ref currLeftHandPos);
            
            attack1yPos = currRightHandPos.y;
        }
    }
    
    // 移动物体位置
    private Task MoveObject(GameObject handObj, int x, int y, ref int2 currentHandPos)
    {
        if (!CheckLimit(x, y)) return Task.CompletedTask;
        int2 newPos = new int2(x, y);
        currentHandPos = newPos;
        return TileManagerSingleton.Instance.MoveObjectToTile(new int2(x,y),handObj);
    }
    private Task MoveObject(GameObject handObj, int2 pos, ref int2 currentHandPos,float time = 0.5f)
    {
        if (!CheckLimit(pos)) return null;
        currentHandPos = pos;
        return TileManagerSingleton.Instance.MoveObjectToTile(pos,handObj,time);
    }
    
    // 头部垂直攻击
    private Task DoHeadVerticalAttack()
    {
        List<int2> attackList = new List<int2>();
        while (currHeadPos.y >= 0)
        {
            int2 targetPos = new int2(currHeadPos.x, currHeadPos.y--);
            if (CheckLimit(targetPos)) attackList.Add(targetPos);
        }
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,2);
        currHeadPos = attackList[^1];
        return TriggerVerticalAttackAnimation(attackList[^1],headObj);
    }

    // 手部垂直攻击
    private Task DoVerticalAttack(GameObject obj, int2 pos,bool value)
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
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList, 2);
        
        if (value) currLeftHandPos = attackList[^1];
        else currRightHandPos = attackList[^1];
        
        return TriggerVerticalAttackAnimation(attackList[^1],obj);
    }

    // 手部水平攻击
    private Task DoHorizontalAttack(GameObject obj, int2 pos,bool isLeft)
    {
        List<int2> attackList = new List<int2>();
        int direction = isLeft ? 1 : -1;
        int startPos = isLeft ? pos.x + 2 : pos.x - 2;
        for (int i = startPos; i >= 0 && i < width; i += direction)
        {
            int2 targetPos = new int2(i, pos.y);
            if (CheckLimit(targetPos)) attackList.Add(targetPos);
        }
        
        GameManagerSingleton.Instance.Player.CheckForDamage(attackList,1);
        
        if (isLeft) currLeftHandPos = attackList[^1];
        else currRightHandPos = attackList[^1];
        
        return TriggerVerticalAttackAnimation(attackList[^1],obj);
    }

    // 手部拍击
    private async Task DoAOEAttackBack(GameObject obj, int2 pos, bool isLeft,bool isBack = true)
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
        await TileManagerSingleton.Instance.AddObjectToTile(tmp,obj);
        if (isLeft) leftHand.PlayPalm();
        else rightHand.PlayPalm();

        if (isBack)
        {
            if (isLeft) currLeftHandPos = pos;
            else currRightHandPos = pos;
            await TileManagerSingleton.Instance.AddObjectToTile(pos,obj);   
        }

        audioManager.AudioUI.clip = audioManager.AudioDic["巨石撞击地面"];
        audioManager.AudioUI.Play();
    }
    
    // placeholder animation
    private Task TriggerVerticalAttackAnimation(int2 targetPos, GameObject obj)
    {
        var TileManager = TileManagerSingleton.Instance;
        TileManager.TileList[TileManager.GetIndexPos(targetPos)].SetObjectToTile(obj);
        Transform currTrans = obj.GetComponent<Transform>();

        Sequence timeline = DOTween.Sequence();
        timeline.Insert(0, currTrans.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.Linear));
        return timeline.Play().AsyncWaitForCompletion();
    }
    
    public Task MoveToReady()
    {
        GameManagerSingleton.Instance.BossLeftAnimator.Play("bossFist");
        GameManagerSingleton.Instance.BossRightAnimator.Play("bossFist");
        return Task.WhenAll(MoveObject(headObj, new int2(width / 2, height - 1), ref currHeadPos,1.5f));
    }
    
    private bool CheckLimit(int2 targetPos)
    {
        return targetPos.x >= 0 && targetPos.x < width && targetPos.y >= 0 && targetPos.y < height;
    }
    private bool CheckLimit(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    public async void CheckForDamage(int value, List<int2> attackList)
    {
        foreach (int2 pos in attackList)
        {
            bool left = CheckPos(currLeftHandPos, pos);
            bool right = CheckPos(currRightHandPos, pos);
            bool head = CheckPos(currHeadPos, pos);
            
            if (left||right||head)
            {
                GameManagerSingleton.Instance.BossHp_UI.OnTakeDamage(value);
                GameManagerSingleton.Instance.BossAnimator.SetTrigger("DamageTrigger");
            }
            
            // GameObject changeObj;
            // if (left) changeObj = GameManagerSingleton.Instance.BossLeftHand;
            // else if (right) changeObj = GameManagerSingleton.Instance.BossRightHand;
            // else if (head) changeObj = GameManagerSingleton.Instance.BossHead;
            // else continue;
            //
            // var tmp = changeObj.GetComponent<SpriteRenderer>().material;
            // changeObj.GetComponent<SpriteRenderer>().material = BossUnderDamageMaterial;
            // await Task.Delay(5000);
            // changeObj.GetComponent<SpriteRenderer>().material = tmp;
        }
    }
    
    private bool CheckPos(int2 targetPos, int2 bossPoss)
    {
        return TileManagerSingleton.Instance.CheckPos(targetPos, bossPoss);
    }

    public Task DieAnimation()
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(PlayAnimationAndWait("DieTrigger", "bossDie", tcs));
        return tcs.Task;
    }
}