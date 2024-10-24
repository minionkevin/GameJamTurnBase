using UnityEngine;
using UnityEngine.UI;

public class FakeNumComponent : MonoBehaviour
{
    public Image FrontSprite;

    public void ChangeSprite(Sprite newSprite)
    {
        FrontSprite.sprite = newSprite;
    }
}
