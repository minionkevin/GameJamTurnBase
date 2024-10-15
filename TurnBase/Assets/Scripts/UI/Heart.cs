using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField]
    Image HeartMask;

    //[SerializeField]
    //Sprite FullSprite;  // 满格心形
    //[SerializeField]
    //Sprite HalfSprite;  // 半格心形

    /// <summary>
    /// 设置血量状态
    /// </summary>
    /// <param name="_value">1_满血；0_空血；-1_半血</param>
    public void SetDisplay(int _value)
    {
        if (_value == 1)
        {
            HeartMask.fillAmount = 1;
            // HeartMask.sprite = FullSprite;
        }
        else if (_value == 0)
        {
            HeartMask.fillAmount = 0;
            // HeartMask.sprite = null;
        }
        else if (_value == -1)
        {
            HeartMask.fillAmount = 0.5f;
            // HeartMask.sprite = HalfSprite;
        }
        else
        {
            Debug.LogError("Error HeartDisplay");
        }
    }
}
