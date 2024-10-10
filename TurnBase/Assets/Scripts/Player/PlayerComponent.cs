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

    private void Update()
    {
        // 注意：这里并没有写完，因为是回合制，现在只是确定能移动
        // 应该是把这一回合的指令存储下来，当玩家按下confirm按钮的时候再统一处理指令
        // 只做测试用
        if (Input.GetKeyDown(KeyCode.W)) HandleMove(new int2(0, -1));
        if (Input.GetKeyDown(KeyCode.A)) HandleMove(new int2(-1, 0));
        if (Input.GetKeyDown(KeyCode.S)) HandleMove(new int2(0, 1));
        if (Input.GetKeyDown(KeyCode.D)) HandleMove(new int2(1, 0));
    }


    public void HandleMove(int2 dir)
    {
        int2 newPos = dir + currPlayerPos;
        if (!CheckLimit(newPos)) return;
        currPlayerPos = newPos;
        TileManagerSingleton.Instance.MovePlayer(currPlayerPos, gameObject);
    }

    private bool CheckLimit(int2 newPos)
    {
        return newPos.x >= 0 && newPos.x < GameStartSingleton.Instance.Width && newPos.y >= 0 && newPos.y < GameStartSingleton.Instance.Height;
    }

}

