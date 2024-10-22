using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAction",menuName = "ScriptableObjects/BossActions",order=1)]
public class BossActionScriptableObject : ScriptableObject
{
    // 连续下劈
    // 左右双掌
    // 全屏激光
    // 地面清扫
    // 范围拍击
    public List<BossActionsData> BossActions = new List<BossActionsData>();
}

[Serializable]
public class BossActionsData
{
    public List<int> BossActions = new List<int>();
}

public class BossInputType
{
    public const int ATTACK10 = 10;
    public const int ATTACK11 = 11;
    public const int ATTACK12 = 12;
    public const int ATTACK13 = 13;
    public const int ATTACK14 = 14;
    public const int ATTACK15 = 15;

    public const int ATTACK20 = 20;
    public const int ATTACK21 = 21;
    public const int ATTACK22 = 22;
    public const int ATTACK23 = 23;
    public const int ATTACK24 = 24;
    public const int ATTACK25 = 25;

    public const int ATTACK30 = 30;
    public const int ATTACK31 = 31;
    public const int ATTACK32 = 32;
    public const int ATTACK33 = 33;
    public const int ATTACK34 = 34;
    public const int ATTACK35 = 35;
    
    public const int ATTACK40 = 40;
    public const int ATTACK41 = 41;
    public const int ATTACK42 = 42;
    public const int ATTACK43 = 43;
    public const int ATTACK44 = 44;
    public const int ATTACK45 = 45;
    
    public const int ATTACK50 = 50;
    public const int ATTACK51 = 51;
    public const int ATTACK52 = 52;
    public const int ATTACK53 = 53;
    public const int ATTACK54 = 54;
    public const int ATTACK55 = 55;

    public const int END = 101;

}
