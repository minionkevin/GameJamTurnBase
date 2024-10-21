using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossInput",menuName = "ScriptableObjects/BossInputs",order=3)]

public class BossInputScriptableObject : ScriptableObject
{
    public List<int> InputList = new List<int>();
}
