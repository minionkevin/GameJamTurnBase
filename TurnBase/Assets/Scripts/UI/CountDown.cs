using UnityEngine;
using TMPro;
using System;

public class CountDown : BaseSingleton<CountDown>
{
    [SerializeField]
    TMP_Text CountDown_Text;
    

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
        curTime = cycleTime;
        timer = 0;
        isTiming = true;

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = curTime / 60;
        int seconds = curTime % 60;
        CountDown_Text.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);

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
