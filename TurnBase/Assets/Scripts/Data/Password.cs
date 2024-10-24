using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Password : MonoBehaviour
{
    public static int[] ItemMark = { 2, 3, 5, 7, 11, 13, 17 };//��ʱ��Ʒ���ܳ���7��
    static int[] times = { 19, 31, 97, 101, 89, 59, 103, 37, 41, 73, 79, 83, 23, 29, 61, 67, 71, 43, 47, 53 };//��ʱ�໥���ݴ���������20��

    static List<int[]> itmeMarkSource = new List<int[]>();
    static List<int[]> timesSource = new List<int[]>();

    /*
     * �ڶ�������
     * public static int[] ItemMark = { 61, 67, 71, 73, 79, 83, 89 };//��ʱ��Ʒ���ܳ���7��
        static int[] times = { 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 2, 3, 5, 7, 11, 13, 17, 97, 101, 103 };//��ʱ�໥���ݴ���������20��
     */

    static List<int> pwTable = new List<int>();
    static List<List<int>> pw12Table = new List<List<int>>();//13���������
    public static Dictionary<int, int> PasswordBook = new Dictionary<int, int>();

    public static void init()
    {
        //�ڶ�������������
        int[] im1 = { 59, 2, 103, 29, 17, 73, 89 };
        int[] im2 = { 73, 61, 43, 71, 37, 83, 101, 7 };
        itmeMarkSource.Add(im1);
        itmeMarkSource.Add(im2);

        int[] t1 = { 79, 83, 19, 23, 5, 31, 37, 41, 11, 43, 47, 53, 67, 71, 3, 7, 13, 97, 101, 61 };
        int[] t2 = { 19, 23, 79, 29, 67, 31, 47, 13, 53, 97, 59, 2, 89, 5, 41, 11, 17, 103, 3 };
        timesSource.Add(t1);
        timesSource.Add(t2);

        //�������ѡ��ȷ����������
        //if(//�ڶ���)
        int seedIndex = 0;
        ItemMark = itmeMarkSource[seedIndex];
        times = timesSource[seedIndex];

        //��ʼ��10���������
        for (int i = 0; i < ItemMark.Length; i++)
        {
            for (int j = 0; j < times.Length; j++)
                pwTable.Add(ItemMark[i] * times[j]);
        }
        //����10�����������������ӳ�����밴����13���������
        for (int i = 0; i < pwTable.Count; i++)
        {
            int x = pwTable[i];
            List<int> To12 = new List<int>();//��λ����λ         
            while (x != 0)
            {
                To12.Add(x % 13);
                x = x / 13;
            }
            To12.Reverse();//��λ����λ
            //��ǰ�����˻����в�����12�ı�����С��12������������Ҫ����
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
        int pw = ItemMark[itemID] * times[GameManagerSingleton.SendCounter++];//���ݵ�ǰ��Ʒ�ʹ���������������Դ˴δ��ݵ�����

        if (!PasswordBook.ContainsKey(itemID))
        {
            PasswordBook.Add(itemID, pw);//��¼��Ʒ�Ͷ�Ӧ����
        }
        else
            PasswordBook[itemID] = pw;//Ӧ�Կ��ܳ��ֵģ�����ʧ�ܣ����»ع�������е�������ٴδ��������µ�����

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
