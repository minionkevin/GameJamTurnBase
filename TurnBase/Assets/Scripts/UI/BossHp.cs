using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHp : MonoBehaviour
{
    [SerializeField]
    Scrollbar HpScroll;

    int maxHp;
    int curHp;

    // 测试用
    // 之后删此调用，在GameManager中调用
    private void OnEnable()
    {
        Setup(40);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="_maxHp"></param>
    public void Setup(int _maxHp)
    {
        curHp = maxHp = _maxHp;
        Update_HpDisplay();
    }

    /// <summary>
    /// 更新血条UI显示
    /// </summary>
    public void Update_HpDisplay()
    {
        HpScroll.size = Mathf.Clamp01((float)curHp / maxHp);
    }

    /// <summary>
    /// 受击
    /// </summary>
    /// <param name="_damageValue">受到的伤害总为正值</param>
    public void OnTakeDamage(int _damageValue)
    {
        curHp -= _damageValue;
        Update_HpDisplay();

        if (curHp <= 0)
        {
            // todo:Boss死亡逻辑
            Debug.Log("Boss Die!");
            GameManagerSingleton.Instance.Boss.DieAnimation();
            return;
        }
    }

}
