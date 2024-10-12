using System.Collections.Generic;
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

    public void AddPlayer(int2 pos,GameObject playerPrefab)
    {
        TileList[GetIndexPos(pos)].AddPlayerToTile(playerPrefab);
    }
    
    public void MovePlayer(int2 pos,GameObject playerPrefab)
    {
        TileList[GetIndexPos(pos)].AddPlayerToTile(playerPrefab);
        
        // Add animation
    }

    private int2 GetIntPos(int index)
    {
        return new int2(index % width, index / width);
    }

    private int GetIndexPos(int2 pos)
    {
        return pos.y * width + pos.x;
    }
}
