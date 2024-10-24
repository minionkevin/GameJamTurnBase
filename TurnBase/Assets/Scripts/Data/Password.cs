using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Password : MonoBehaviour
{
    static int[] ItemMark = { 2, 3, 5, 7, 11, 13, 17 };//暂时物品不能超过7种
    static int[] times = { 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103 };//暂时相互传递次数不超过20次
    List<int> pwTable = new List<int>();
    public static Dictionary<int, int> PasswordBook = new Dictionary<int, int>();

    public void init()
    {
        //玩家对齐
        //currentseed = randomSeed[0];

        for (int i = 0; i < ItemMark.Length; i++)
        {
            for (int j = 0; j < times.Length; j++)
                pwTable.Add(ItemMark[i] * times[j]);
        }

    }
    public static void ItemPasswordRenew(int itemID)
    {
        int pw = ItemMark[itemID] * times[GameManagerSingleton.SendCounter++];//根据当前物品和传递次数，生成针对此次传递的密码

        if (!PasswordBook.ContainsKey(itemID))
        {
            PasswordBook.Add(itemID, pw);//记录物品和对应密码
        }
        else
            PasswordBook[itemID] = pw;//应对可能出现的，传出失败，重新回归玩家手中的情况，再次传递生成新的密码

    }

}
