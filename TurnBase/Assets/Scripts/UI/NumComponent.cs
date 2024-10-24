using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumComponent : MonoBehaviour
{
    public List<Sprite> NumImages = new List<Sprite>();
    public Image NumImage;
    private TakeItemComponent takeItemComponent;
    private int id;
    
    // for take component 
    public void Setup(int num,TakeItemComponent takeItemComponent)
    {
        NumImage.sprite = NumImages[num];
        this.takeItemComponent = takeItemComponent;
        id = num+1;
    }
    
    // for password
    public void Setup(int num)
    {
        NumImage.sprite = NumImages[num-1];
        id = num;
    }

    public void UpdateNumImageRandomAll(RectTransform NumRect)
    {
        int[] numIndex = new int[NumImages.Count];
        for (int i = 0; i < numIndex.Length; i++)
        {
            numIndex[i] = i;
        }
        
        int index = 0;
        int temp = 0;
        for (int i = 0; i < numIndex.Length; i++)
        {
            index = Random.Range(0, numIndex.Length - i);
            if (index != i)
            {
                temp = numIndex[i];
                numIndex[i] = numIndex[index];
                numIndex[index] = temp;
            }
        }
        for (int i = 0; i < NumRect.childCount; i++)
        {
            NumRect.GetChild(i).GetComponent<NumComponent>().NumImage.sprite = NumImages[numIndex[i]];
        }
    }

    public void SendInfo()
    {
        takeItemComponent.AddToPassword(id);
    }

}
