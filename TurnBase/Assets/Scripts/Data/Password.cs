using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Password : MonoBehaviour
{
    public static int[] ItemMark = { 2, 3, 5, 7, 11, 13, 17 };//暂时物品不能超过7种
    static int[] times = { 19, 31, 97, 101, 89, 59, 103, 37, 41, 73, 79, 83, 23, 29, 61, 67, 71, 43, 47, 53 };//暂时相互传递次数不超过20次

    static List<int[]> itmeMarkSource = new List<int[]>();
    static List<int[]> timesSource = new List<int[]>();

    /*
     * 第二套种子
     * public static int[] ItemMark = { 61, 67, 71, 73, 79, 83, 89 };//暂时物品不能超过7种
        static int[] times = { 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 2, 3, 5, 7, 11, 13, 17, 97, 101, 103 };//暂时相互传递次数不超过20次
     */

    static List<int> pwTable = new List<int>();
    static List<List<int>> pw12Table = new List<List<int>>();//13进制密码表
    public static Dictionary<int, int> PasswordBook = new Dictionary<int, int>();

    public static void init()
    {
        //第二三套密码种子
        int[] im1 = { 59, 2, 103, 29, 17, 73, 89 };
        int[] im2 = { 73, 61, 43, 71, 37, 83, 101, 7 };
        itmeMarkSource.Add(im1);
        itmeMarkSource.Add(im2);

        int[] t1 = { 79, 83, 19, 23, 5, 31, 37, 41, 11, 43, 47, 53, 67, 71, 3, 7, 13, 97, 101, 61 };
        int[] t2 = { 19, 23, 79, 29, 67, 31, 47, 13, 53, 97, 59, 2, 89, 5, 41, 11, 17, 103, 3 };
        timesSource.Add(t1);
        timesSource.Add(t2);

        //根据玩家选择，确定密码种子
        //if(//第二套)
        int seedIndex = 0;
        ItemMark = itmeMarkSource[seedIndex];
        times = timesSource[seedIndex];

        //初始化10进制密码表
        for (int i = 0; i < ItemMark.Length; i++)
        {
            for (int j = 0; j < times.Length; j++)
                pwTable.Add(ItemMark[i] * times[j]);
        }
        //根据10进制密码表，生成用于映射密码按键的13进制密码表
        for (int i = 0; i < pwTable.Count; i++)
        {
            int x = pwTable[i];
            List<int> To12 = new List<int>();//低位到高位         
            while (x != 0)
            {
                To12.Add(x % 13);
                x = x / 13;
            }
            To12.Reverse();//高位到低位
            //当前质数乘积表中不存在12的倍数或小于12的数，否则需要处理
            pw12Table.Add(To12);
        }

    }
    void convertTo12()
    {

    }

    public static void ItemPasswordRenew(int itemID)
    {
        if (GameManagerSingleton.SendCounter > 20)
            GameManagerSingleton.SendCounter = 1;
        int pw = ItemMark[itemID] * times[GameManagerSingleton.SendCounter++];//根据当前物品和传出次数，生成针对此次传递的密码

        if (!PasswordBook.ContainsKey(itemID))
        {
            PasswordBook.Add(itemID, pw);//记录物品和对应密码
        }
        else
            PasswordBook[itemID] = pw;//应对可能出现的，传出失败，重新回归玩家手中的情况，再次传递生成新的密码

    }

    public static bool pwdVerified(int pwd)
    {
        return pwTable.Contains(pwd); 
    }

    public static List<int> get12PW(int itemID)
    {
        int pwd = PasswordBook[itemID];
        for (int i = 0; i < pwTable.Count; i++)
        {
            if (pwTable[i] == pwd)
                return pw12Table[i];
        }

        return null;
    }

    public static int To10(List<int> input)
    {
        float result = 0;
        for (int i = input.Count - 1; i >= 0; i--)
        {
            result += input[i] * Mathf.Pow(13, input.Count - i - 1);
        }
        return (int)result;
    }


    public static bool checkReceived(int time)
    {
        foreach (int t in GameManagerSingleton.ReceivedTimes)
        {
            if (t == time)
                return true;
        }
        return false; ;
    }

    void reGeneratePasswordSeeds()
    {
        int l = ItemMark.Length;
        int t = times.Length;

        int[] all = new int[ItemMark.Length + times.Length];
        for (int i = 0; i < ItemMark.Length; i++)
        {
            all[i] = ItemMark[i];
        }
        for (int i = 0; i < times.Length; i++)
        {
            all[i + l] = times[i];
        }

        List<int> tempItemMark = new List<int>();
        List<int> tempTimes = new List<int>();
        for (int ii = 0; ii < l; ii++)
        {
            tempItemMark.Add(all[Random.Range(0, l)]); ;
        }
        for (int a = 0; a < all.Length; a++)
        {
            if (!tempItemMark.Contains(times[a]))
                tempTimes.Add(all[a]);
        }
        ItemMark = tempItemMark.ToArray();
        times = tempTimes.ToArray();

        int index = 0;
        int temp = 0;
        for (int i = 0; i < t; i++)
        {
            index = Random.Range(0, t-i);
            if (index != i)
            {
                temp = times[i];
                times[i] = times[index];
                times[index] = temp;
            }
        }
    }

}
