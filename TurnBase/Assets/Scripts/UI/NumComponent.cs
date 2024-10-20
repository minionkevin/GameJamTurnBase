using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumComponent : MonoBehaviour
{
    public TextMeshProUGUI NumLabel;
    public List<Sprite> NumImages = new List<Sprite>();
    public Image NumImage;
    private TakeItemComponent takeItemComponent;
    private int id;
    
    public void Setup(int num,TakeItemComponent takeItemComponent)
    {
        NumLabel.text = num.ToString();
        NumImage.sprite = NumImages[num];
        this.takeItemComponent = takeItemComponent;
        id = num;
    }

    public void SendInfo()
    {
        takeItemComponent.AddToPassword(id);
    }

}
