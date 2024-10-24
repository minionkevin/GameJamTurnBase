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

    public void PasswordSetup(int num)
    {
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
        takeItemComponent.AddToPassword(id,NumImage.sprite);
    }

    public void SendPasswordInfo()
    {
        // todo 这里写对密码界面收到玩家输入逻辑
    }

}
