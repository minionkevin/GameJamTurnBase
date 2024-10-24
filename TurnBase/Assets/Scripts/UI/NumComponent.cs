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
    

    public void SendInfo()
    {
        takeItemComponent.AddToPassword(id);
    }

}
