using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "ScriptableObjects/Items",order=2)]
public class ItemScriptableObject : ScriptableObject
{
    public List<ItemData> ItemDatas = new List<ItemData>();
}

[Serializable]
public class ItemData
{
    public int Id;
    public int Amount;
}

// 0 长剑
// 1 锤子
// 2 护罩
// 3 葫芦
