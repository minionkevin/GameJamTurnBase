using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class TileManagerSingleton : BaseSingleton<TileManagerSingleton>
{
    public GameObject TilePrefab;
    public Transform ContainerRect;

    public List<TileComponent> TileList = new List<TileComponent>();
    
    private int width;
    private int height;
    
    public void Setup(int xCount, int yCount)
    {
        width = xCount;
        height = yCount;

        GenerateTiles(1,0.1f);
    }
    
    void GenerateTiles(float tileSize, float tileSpacing)
    {
        float totalWidth = (width - 1) * (tileSize + tileSpacing);   // 计算网格总宽度
        float totalHeight = (height - 1) * (tileSize + tileSpacing); // 计算网格总高度

        float offsetX = (width % 2 == 0) ? (totalWidth / 2f) - (tileSize + tileSpacing) / 2f:  (totalWidth / 2f);
        float offsetY = (height % 2 == 0) ? (totalHeight / 2f) - (tileSize + tileSpacing) / 2f : (totalHeight / 2f) ;

        for (int i = 0; i < width * height; i++)
        {
            var tile = Instantiate(TilePrefab, ContainerRect);

            // 获取当前 Tile 的 grid 位置
            int2 gridPos = GetIntPos(i);

            // 计算每个 Tile 的实际位置，使其居中
            float posX = gridPos.x * (tileSize + tileSpacing) - offsetX;
            float posY = gridPos.y * (tileSize + tileSpacing) - offsetY;

            tile.GetComponent<Transform>().position = new Vector3(posX, posY, 0);
        
            var tileComponent = tile.GetComponent<TileComponent>();
            TileList.Add(tileComponent);
            tileComponent.Setup(i, gridPos);
        }
    }

    public Task AddObjectToTile(int2 pos,GameObject objectPrefab)
    {
        return TileList[GetIndexPos(pos)].AddObjectToTile(objectPrefab);
    }
    
    // 这个函数之后可以在加一个bool为 isPlayer
    // 方便做动画，boss和player的移动动画应该是不一样的
    public Task MoveObjectToTile(int2 pos,GameObject objectPrefab)
    {
        return TileList[GetIndexPos(pos)].AddObjectToTile(objectPrefab);
    }

    public async void MoveObjectToTile(int2 pos, GameObject objectPrefab, float time = 0.5f, Ease ease = Ease.Linear)
    {
        await TileList[GetIndexPos(pos)].AddObjectToTile(objectPrefab, time, ease);
    }

    private int2 GetIntPos(int index)
    {
        return new int2(index % width, index / width);
    }

    public int GetIndexPos(int2 pos)
    {
        return pos.y * width + pos.x;
    }

    /// <summary>
    /// 核对要检测的位置是否存放某物体
    /// </summary>
    /// <param name="posForCheck">要查看的位置</param>
    /// <param name="objPos">物体的位置</param>
    /// <returns></returns>
    public bool CheckPos(int2 posForCheck,int2 objPos)
    {
        return posForCheck.Equals(objPos);
    }
}
