using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Password : MonoBehaviour
{
    //密码种子，用于生成游戏中传递物品时使用的密码
    public static int[] ItemMark = new int[7];//暂时物品不能超过7种
    static int[] times = new int[20];//暂时相互传递次数不超过20次

    //备选密码种子库
    static List<int[]> itmeMarkSource = new List<int[]>();
    static List<int[]> timesSource = new List<int[]>();
    //选择序列，用于决定使用哪一套密码种子
    public static Dictionary<int, int[]> SeedSelectSequence = new Dictionary<int, int[]>();//随机种子选择器

    static List<int> pwTable = new List<int>();
    static List<List<int>> pw12Table = new List<List<int>>();//13进制密码表
    public static Dictionary<int, int> PasswordBook = new Dictionary<int, int>();

    public static void init()
    {
        //初始化种子库和选择序列，未来改读表
        initPasswordSeedAndSequences();
        
        //selectPWSeed(2);//此行仅用于快速测试，删除不影响正常流程游戏
    }

    static void initPasswordSeedAndSequences()
    {
        //10套密码种子
        int[] im1 = { 59, 2, 103, 29, 17, 73, 89 };
        int[] im2 = { 73, 61, 43, 71, 37, 101, 7 };
        int[] im3 = { 2, 3, 5, 7, 11, 13, 17 };
        int[] im4 = { 89, 5, 29, 101, 13, 47, 59 };
        int[] im5 = { 79, 7, 61, 97, 47, 11, 2 };
        int[] im6 = { 67, 71, 43, 3, 23, 19, 73 };
        int[] im7 = { 37, 67, 5, 19, 23, 2, 41 };
        int[] im8 = { 71, 3, 43, 101, 7, 73, 13 };
        int[] im9 = { 11, 2, 23, 41, 73, 19, 5 };
        int[] im10 = { 23, 29, 79, 7, 89, 59, 13 };

        itmeMarkSource.Add(im1);
        itmeMarkSource.Add(im2);
        itmeMarkSource.Add(im3);
        itmeMarkSource.Add(im4);
        itmeMarkSource.Add(im5);
        itmeMarkSource.Add(im6);
        itmeMarkSource.Add(im7);
        itmeMarkSource.Add(im8);
        itmeMarkSource.Add(im9);
        itmeMarkSource.Add(im10);

        int[] t1 = { 79, 83, 19, 23, 5, 31, 37, 41, 11, 43, 47, 53, 67, 71, 3, 7, 13, 97, 101, 61 };
        int[] t2 = { 19, 23, 79, 29, 67, 31, 47, 13, 53, 97, 59, 2, 89, 5, 41, 11, 17, 103, 83, 3 };
        int[] t3 = { 19, 31, 97, 101, 89, 59, 103, 37, 41, 73, 79, 83, 23, 29, 61, 67, 71, 43, 47, 53};
        int[] t4 = { 73, 79, 61, 7, 37, 3, 11, 17, 19, 71, 67, 53, 97, 103, 2, 83, 23, 31, 43, 41 };
        int[] t5 = { 23, 29, 67, 71, 3, 37, 41, 73, 19, 31, 5, 83, 89, 59, 43, 53, 13, 101, 17, 103 };
        int[] t6 = { 59, 13, 97, 103, 29, 7, 11, 61, 2, 89, 47, 53, 5, 101, 37, 41, 79, 83, 17, 31 };
        int[] t7 = { 3, 83, 103, 73, 71, 43, 79, 97, 101, 47, 89, 61, 31, 53, 29, 17, 7, 11, 13, 59 };
        int[] t8 = { 83, 103, 67, 2, 89, 37, 41, 23, 97, 47, 11, 19, 31, 79, 61, 5, 17, 53, 29, 59 };
        int[] t9 = { 89, 59, 43, 53, 13, 7, 61, 97, 103, 29, 101, 67, 71, 3, 47, 83, 37, 31, 79, 17 };
        int[] t10 = { 31, 5, 47, 11, 67, 71, 83, 61, 97, 2, 43, 53, 101, 41, 73, 19, 17, 103, 3, 37 };
        timesSource.Add(t1);
        timesSource.Add(t2);
        timesSource.Add(t3);
        timesSource.Add(t4);
        timesSource.Add(t5);
        timesSource.Add(t6);
        timesSource.Add(t7);
        timesSource.Add(t8);
        timesSource.Add(t9);
        timesSource.Add(t10);

        //随机种子选择器
        int[] sequence1 = { 9, 4, 1, 6, 12, 5, 8, 11, 3, 7, 2, 10 };
        int[] sequence2 = { 12, 6, 1, 7, 2, 10, 3, 11, 8, 4, 5, 9 };
        int[] sequence3 = { 3, 11, 6, 10, 1, 8, 5, 12, 7, 9, 2, 4 };
        int[] sequence4 = { 8, 11, 3, 7, 9, 4, 1, 6, 12, 5, 2, 10 };
        int[] sequence5 = { 6, 1, 7, 2, 10, 3, 12, 11, 8, 4, 5, 9 };
        int[] sequence6 = { 9, 2, 4, 3, 11, 6, 10, 1, 8, 5, 12, 7 };
        int[] sequence7 = { 11, 3, 7, 2, 9, 4, 1, 6, 12, 5, 8, 10 };
        int[] sequence8 = { 1, 7, 2, 10, 3, 5, 9, 12, 6, 11, 8, 4 };
        int[] sequence9 = { 6, 10, 1, 8, 5, 12, 7, 3, 11, 9, 2, 4 };
        int[] sequence10 = { 12, 7, 8, 5, 11, 3, 6, 10, 1, 9, 2, 4 };
        SeedSelectSequence.Add(1, sequence1);
        SeedSelectSequence.Add(2, sequence2);
        SeedSelectSequence.Add(3, sequence3);
        SeedSelectSequence.Add(4, sequence4);
        SeedSelectSequence.Add(5, sequence5);
        SeedSelectSequence.Add(6, sequence6);
        SeedSelectSequence.Add(7, sequence7);
        SeedSelectSequence.Add(8, sequence8);
        SeedSelectSequence.Add(9, sequence9);
        SeedSelectSequence.Add(10, sequence10);
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

    public static void selectPWSeed(int seedIndex)
    {
        ItemMark = itmeMarkSource[seedIndex-1];
        times = timesSource[seedIndex-1];
        pwTable = new List<int>();
        pw12Table = new List<List<int>>();

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
    public static int SequenceExist(List<int> input)
    {
        for (int i = 1; i <= SeedSelectSequence.Count; i++)
        {
            for (int j = 0; j < input.Count; j++)
            {
                if (input[j] == SeedSelectSequence[i][j])
                {
                    if (j == input.Count - 1)
                    {
                        return i;
                    }
                }
                else
                    break;
            }      
        }
        return -1;

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
