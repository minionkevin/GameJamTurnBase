using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Password : MonoBehaviour
{
    static int[] ItemMark = { 2, 3, 5, 7, 11, 13, 17 };//��ʱ��Ʒ���ܳ���7��
    static int[] times = { 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103 };//��ʱ�໥���ݴ���������20��
    List<int> pwTable = new List<int>();
    public static Dictionary<int, int> PasswordBook = new Dictionary<int, int>();

    public void init()
    {
        //��Ҷ���
        //currentseed = randomSeed[0];

        for (int i = 0; i < ItemMark.Length; i++)
        {
            for (int j = 0; j < times.Length; j++)
                pwTable.Add(ItemMark[i] * times[j]);
        }

    }
    public static void ItemPasswordRenew(int itemID)
    {
        int pw = ItemMark[itemID] * times[GameManagerSingleton.SendCounter++];//���ݵ�ǰ��Ʒ�ʹ��ݴ�����������Դ˴δ��ݵ�����

        if (!PasswordBook.ContainsKey(itemID))
        {
            PasswordBook.Add(itemID, pw);//��¼��Ʒ�Ͷ�Ӧ����
        }
        else
            PasswordBook[itemID] = pw;//Ӧ�Կ��ܳ��ֵģ�����ʧ�ܣ����»ع�������е�������ٴδ��������µ�����

    }

}
