using System.Collections.Generic;
using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    Transform HeartContent;
    [SerializeField]
    Heart HeartPref;

    // 由于半颗心的存在，存储值为输入值*2；
    int maxHp;
    int curHp;
    List<Heart> HeartsList;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="_realMaxHp">未缩放的初始血量</param>
    public void Setup(int _realMaxHp)
    {
        curHp = maxHp = _realMaxHp * 2;
        HeartsList = new List<Heart>();

        for (int i = 0; i < _realMaxHp; i++)
        {
            GameObject hpPref = Instantiate(HeartPref.gameObject, HeartContent);
            HeartsList.Add(hpPref.GetComponent<Heart>());
        }
    }

    public void Clear()
    {
        foreach (var item in HeartsList)
        {
            Destroy(item.gameObject);
        }
        HeartsList.Clear();
    }

    /// <summary>
    /// 更新血条UI显示
    /// 半格血逻辑
    /// </summary>
    private void Update_HpDisplay()
    {
        for (int i = 0; i < maxHp/2; i++)
        {
            if (i < curHp / 2)  HeartsList[i].SetDisplay(1);
            else if (i > curHp / 2) HeartsList[i].SetDisplay(0);
            else HeartsList[i].SetDisplay(curHp % 2 == 1 ? -1 : 0);
        }
        
    }

    /// <summary>
    /// 受击
    /// </summary>
    /// <param name="_damageValue">传入已经缩放后的伤害值（0.5->1）</param>
    public void OnTakeDamage(int _damageValue)
    {
        curHp -= _damageValue;
        if (curHp <= 0) curHp = 0;
        Update_HpDisplay();

        if (curHp > 0) return;
        GameManagerSingleton.Instance.HandlePlayerDie(true);
    }

    /// <summary>
    /// 恢复
    /// </summary>
    /// <param name="_recoverValue">传入已经缩放后的恢复值（2->4）</param>
    public void OnGetRecovery(int _recoverValue)
    {
        curHp += _recoverValue;
        curHp = Mathf.Min(curHp,maxHp);

        Update_HpDisplay();
    }

}
