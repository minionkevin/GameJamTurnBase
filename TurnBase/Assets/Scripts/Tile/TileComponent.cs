using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    // 注意：alpha=0，用之前需要先调整
    public GameObject EffectLayer;
    public Transform ObjectLayerRect;
    // Debug only
    public TextMeshProUGUI PosLabel;

    private int tileIndex;
    private int2 tilePos;

    public void Setup(int index,int2 tilePos)
    {
        tileIndex = index;
        this.tilePos = tilePos;
        PosLabel.text = "(" + tilePos.x + "," + tilePos.y + ")";
    }

    public async void AddObjectToTile(GameObject objPrefab)
    {
        objPrefab.GetComponent<Transform>().SetParent(ObjectLayerRect);
        // Add animation
        Transform trans = objPrefab.GetComponent<Transform>();
        await trans.DOLocalMove(Vector3.zero, 0.5f).AsyncWaitForCompletion();
    }

    public void SetObjectToTile(GameObject objPrefab)
    {
        objPrefab.GetComponent<Transform>().SetParent(ObjectLayerRect);
    }
}
