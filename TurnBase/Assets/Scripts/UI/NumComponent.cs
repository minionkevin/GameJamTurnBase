using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumComponent : MonoBehaviour
{
    public Image NumImage;
    private TakeItemComponent takeItemComponent;
    private int id;
    
    // for take component 
    public void Setup(int num,TakeItemComponent takeItemComponent)
    {
        this.takeItemComponent = takeItemComponent;
        id = num+1;
    }
    
    // for password
    public void Setup(int num)
    {
        // NumImage.sprite = NormalNumImages[num-1];
        id = num;
    }
    

    public void SendInfo()
    {
        takeItemComponent.AddToPassword(id,NumImage.sprite);
    }

}
