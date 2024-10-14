using UnityEngine;
using TMPro;
using System;

public class CountDown : BaseSingleton<CountDown>
{
    [SerializeField]
    TMP_Text CountDown_Text;

    // 计时循环
    [HideInInspector]
    public int CountDownCycle = 60;

    // 计时及状态
    int curTime = 0;
    float timer = 0;
    bool isTiming = false;

    public Action OnTimerEnd;   // 计时结束后的事件

    /// <summary>
    /// 初始化计时器
    /// </summary>
    public void Setup(int cycleTime)
    {
        curTime = CountDownCycle = cycleTime;
        timer = 0;
        isTiming = true;

        UpdateTimerDisplay();
    }

    public void UpdateTimerDisplay()
    {
        CountDown_Text.text = curTime.ToString("00:00");

        if (curTime <= 0)
        {
            isTiming = false;
            // 计时结束，（执行对战）
            OnTimerEnd?.Invoke();
        }
    }


    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void SetTimerPause()
    {
        isTiming = false;
    }

    private void Update()
    {
        if (!isTiming)
        {
            return;
        }

        timer += Time.deltaTime;
        // todo:使用假的计时间隔
        if (timer > 1)
        {
            timer = 0;
            curTime--;
            UpdateTimerDisplay();
        }
    }

}
