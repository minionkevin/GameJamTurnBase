using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class TileComponent : MonoBehaviour
{
    // 注意：alpha=0，用之前需要先调整
    public GameObject EffectLayer;
    public RectTransform PlayerLayerRect;
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

    public void AddPlayer(GameObject playerPrefab)
    {
        playerPrefab.GetComponent<RectTransform>().SetParent(PlayerLayerRect);
        playerPrefab.transform.localPosition = Vector3.zero;
    }
}
