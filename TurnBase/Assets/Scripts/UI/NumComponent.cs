using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumComponent : MonoBehaviour
{
    public TextMeshProUGUI NumLabel;
    
    private TakeItemComponent takeItemComponent;
    private int id;


    public void Setup(int num,TakeItemComponent takeItemComponent)
    {
        NumLabel.text = num.ToString();
        this.takeItemComponent = takeItemComponent;
        id = num;
    }

    public void SendInfo()
    {
        takeItemComponent.AddToPassword(id);
    }

}
