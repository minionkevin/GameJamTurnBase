using UnityEngine;
using TMPro;
using System;

public class CountDown : BaseSingleton<CountDown>
{
    [SerializeField]
    TMP_Text CountDown_Text;

    // 计时循环
    public int CountDownCycle = 60;

    // 计时及状态
    int curTime = 0;
    float timer = 0;
    bool isTiming = false;

    public Action OnTimerEnd;

    private void OnEnable()
    {
        SetTimerInit();
    }

    private void Update()
    {
        if (!isTiming)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
            curTime--;
            UpdateTimerDisplay();
        }
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
    /// 初始化计时器
    /// </summary>
    public void SetTimerInit()
    {
        curTime = CountDownCycle;
        timer = 0;
        isTiming = true;

        UpdateTimerDisplay();
    }


    /// <summary>
    /// 暂停计时器
    /// </summary>
    public void SetTimerPause()
    {
        isTiming = false;
    }

}
