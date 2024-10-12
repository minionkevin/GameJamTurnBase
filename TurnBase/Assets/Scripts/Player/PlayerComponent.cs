using Unity.Mathematics;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    // 每次玩家移动需更新
    private int2 currPlayerPos;
    
    public void Setup(int2 playerPos)
    {
        currPlayerPos = playerPos;
    }

    public void HandleMove(int2 dir)
    {
        Debug.LogError("MOVE");
        int2 newPos = dir + currPlayerPos;
        if (!CheckLimit(newPos)) return;
        currPlayerPos = newPos;
        TileManagerSingleton.Instance.MoveObjectToTile(currPlayerPos, gameObject);
    }

    private bool CheckLimit(int2 newPos)
    {
        return newPos.x >= 0 && newPos.x < GameManagerSingleton.Instance.Width && newPos.y >= 0 && newPos.y < GameManagerSingleton.Instance.Height;
    }

}

