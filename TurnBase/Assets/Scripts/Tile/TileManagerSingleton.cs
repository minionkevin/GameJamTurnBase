using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TileManagerSingleton : BaseSingleton<TileManagerSingleton>
{
    public GameObject TilePrefab;
    public RectTransform ContainerRect;

    public List<TileComponent> TileList = new List<TileComponent>();
    
    private int width;
    private int height;
    
    public void Setup(int xCount, int yCount)
    {
        this.width = xCount;
        this.height = yCount;
        
        for (int i = 0; i < xCount * yCount; i++)
        {
            var tile = Instantiate(TilePrefab, ContainerRect);
            var tileComponent = tile.GetComponent<TileComponent>();
            TileList.Add(tileComponent);
            tileComponent.Setup(i,GetIntPos(i));
        }
    }

    public void AddPlayer(int2 pos,GameObject playerPrefab)
    {
        TileList[GetIndexPos(pos)].AddPlayer(playerPrefab);
    }
    
    public void MovePlayer(int2 pos,GameObject playerPrefab)
    {
        TileList[GetIndexPos(pos)].AddPlayer(playerPrefab);
        
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
