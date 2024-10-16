using System;
using System.Threading.Tasks;
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

    public int GetTileIndex()
    {
        return tileIndex;
    }

    //[Obsolete]
    //public Task AddObjectToTile(GameObject objPrefab)
    //{
    //    objPrefab.GetComponent<Transform>().SetParent(ObjectLayerRect);
    //    // Add animation
    //    Transform trans = objPrefab.GetComponent<Transform>();
    //    return trans.DOLocalMove(Vector3.zero, 0.5f).AsyncWaitForCompletion();
    //}

    /// <summary>
    /// 移动某物体到指定位置（设置时间和曲线）
    /// </summary>
    /// <param name="objPrefab">移动的物体</param>
    /// <param name="time">移动时间间隔</param>
    /// <param name="ease">移动曲线</param>
    /// <returns></returns>
    public Task AddObjectToTile(GameObject objPrefab,float time = 0.5f,Ease ease = Ease.Linear)
    {
        objPrefab.GetComponent<Transform>().SetParent(ObjectLayerRect);
        // Add animation
        Transform trans = objPrefab.GetComponent<Transform>();
        return trans.DOLocalMove(Vector3.zero, time).SetEase(ease).AsyncWaitForCompletion();
    }

    public void SetObjectToTile(GameObject objPrefab)
    {
        objPrefab.GetComponent<Transform>().SetParent(ObjectLayerRect);
    }
}
