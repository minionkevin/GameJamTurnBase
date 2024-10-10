using Unity.Mathematics;
using UnityEngine;

public class GameStartSingleton : BaseSingleton<GameStartSingleton>
{
    public int Width = 7;
    public int Height = 6;
    
    
    // 有了config之后从scriptableobject read
    public int2 PlayerStartPos;
    
    public GameObject PlayerPrefab;
    
    
    
    void Start()
    {
        TileManagerSingleton.Instance.Setup(Width, Height);
        var player = Instantiate(PlayerPrefab);
        var playerComponent = player.GetComponent<PlayerComponent>();
        playerComponent.Setup(PlayerStartPos);
        
        TileManagerSingleton.Instance.AddPlayer(PlayerStartPos,player);
    }
    
}
